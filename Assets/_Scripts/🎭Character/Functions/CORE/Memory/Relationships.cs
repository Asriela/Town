using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using UnityEngine;
using UnityEngine.TextCore.Text;


[Serializable]
public class CharacterFloatPair
{
    public Character targetCharacter; // The character this relationship is about
    public float relationshipValue;   // The relationship value (e.g., positive, neutral, or negative)
}

[Serializable]
public class CharacterRelationshipFloatPair
{
    public Character character;                      // The character whose relationships are stored
    public List<CharacterFloatPair> relationships = new(); // List of relationships with other characters
}

public class InteractionEffect
{
    public float value;
    public SocializeType type;
    public int amountOfRepeatsToday;
    //TODO: add long lifespan so effects even when dimminished dont last forever

    public InteractionEffect(SocializeType type, float value)
    {
        this.value = value;
        this.type = type;
        this.amountOfRepeatsToday = 1;
    }
}

public class Relationships : MonoBehaviour
{
    private Character _character;

    public void Initialize(Character character)
    {
        _character = character;
        WorldManager.Instance.OnMidnight += MoveTodaysInteractionEffectsToThePast;
    }

    [SerializeField]
    private List<CharacterRelationshipFloatPair> _characterRelationships = new();

    public List<InteractionEffect> TodaysInteractionEffects { get; set; } = new();

    public List<InteractionEffect> PastInteractionEffects { get; set; } = new();

    public float AddInteractionEffect(SocializeType socializeType, Character giverOfInteraction)
    {
        string responseDialogue = "";
        float effectFromInteraction = 0;
        // Check if there's already an InteractionEffect of this type
        var existingEffect = TodaysInteractionEffects.FirstOrDefault(effect => effect.type == socializeType);

        if (existingEffect != null)
        {
            // Increment the repeat counter
            existingEffect.amountOfRepeatsToday++;
            effectFromInteraction = CalculateInteractionEffectValue(socializeType, existingEffect.amountOfRepeatsToday,
                GetRelationshipWith(_character, giverOfInteraction), out responseDialogue);
            existingEffect.value = effectFromInteraction;


        }
        else
        {
            effectFromInteraction = CalculateInteractionEffectValue(socializeType, 1,
                GetRelationshipWith(_character, giverOfInteraction), out responseDialogue);

            // Create a new InteractionEffect and calculate its value
            var newEffect = new InteractionEffect(
                socializeType, effectFromInteraction
             );


            TodaysInteractionEffects.Add(newEffect);
        }
         
         
        if (responseDialogue != "")
        { _character.Ui.Speak(responseDialogue); }

        RecalculateMyRelationshipWithEveryone();

        return effectFromInteraction;
    }

    public float AddInteractionEffect(SocializeType socializeType, Character giverOfInteraction, float interactionEffect)
    {
        string responseDialogue = "";
        float effectFromInteraction = 0;
        // Check if there's already an InteractionEffect of this type
        var existingEffect = TodaysInteractionEffects.FirstOrDefault(effect => effect.type == socializeType);

        if (existingEffect != null)
        {
            // Increment the repeat counter
            existingEffect.amountOfRepeatsToday++;

            existingEffect.value += interactionEffect / 2;


        }
        else
        {


            // Create a new InteractionEffect and calculate its value
            var newEffect = new InteractionEffect(
                socializeType, interactionEffect /2
            );


            TodaysInteractionEffects.Add(newEffect);
        }


        if (responseDialogue != "")
        { _character.Ui.Speak(responseDialogue); }
        RecalculateMyRelationshipWithEveryone();

        return effectFromInteraction;
    }

    private float CalculateInteractionEffectValue(SocializeType socializeType, int todaysRepeats, float relationshipValueWithGiverOfInteraction, out string responseDialogue)
    {
        float effectValue = 0f;
        responseDialogue = "";
        switch (socializeType)
        {
            case SocializeType.hug:
                if (relationshipValueWithGiverOfInteraction >= (float)ViewTowards.veryPositive)
                {
                    if (todaysRepeats <= 2)
                    {
                        responseDialogue = "ahh thanks that's a nice hug";
                        effectValue = (float)ViewTowards.positive;
                    }
                    else
                    {
                        responseDialogue = "why do you keep hugging me, its weird";
                        effectValue = (float)ViewTowards.negative;
                    }
                }
                else
                {
                    responseDialogue = "Why are you hugging me...we arnt that close.";
                    effectValue = ((float)ViewTowards.veryNegative) * todaysRepeats;
                }
                break;
            case SocializeType.greet:
                if (relationshipValueWithGiverOfInteraction <= (float)ViewTowards.veryNegative)
                {
                    responseDialogue = "ug you again..";
                    effectValue = ((float)ViewTowards.negative) * todaysRepeats;

                }
                else
                {
                    if (todaysRepeats <= 1)
                    {
                        responseDialogue = "Good day to you too!";
                        effectValue = (float)ViewTowards.positive;
                    }
                    else
                    {
                        responseDialogue = "You already greeted me..";
                        effectValue = (float)ViewTowards.negative;
                    }
                }
                break;
            case SocializeType.smallTalk:
                if (relationshipValueWithGiverOfInteraction <= (float)ViewTowards.veryNegative)
                {
                    responseDialogue = "ug you again..";
                    effectValue = ((float)ViewTowards.negative) * todaysRepeats;

                }
                else
                {
                    if (todaysRepeats <= 4)
                    {
                        responseDialogue = "So did you hear about...";
                        effectValue = (float)ViewTowards.positive;
                    }
                    else
                    {
                        responseDialogue = "Haven't we spoken enough for the day?";
                        effectValue = (float)ViewTowards.negative;
                    }
                }
                break;
            case SocializeType.insult:
                if (relationshipValueWithGiverOfInteraction <= (float)ViewTowards.negative)
                {
                    responseDialogue = "Yeah sounds like something an asshole like you would say.";
                    effectValue = ((float)ViewTowards.veryNegative) * todaysRepeats;

                }
                else
                {
                    if (todaysRepeats <= 1)
                    {
                        responseDialogue = "Surely you don't mean that...";
                        effectValue = (float)ViewTowards.negative;
                    }
                    else
                    {
                        responseDialogue = "I can't believe you have done this!";
                        effectValue = ((float)ViewTowards.veryNegative) * todaysRepeats;
                    }
                }
                break;
        }


        return effectValue / 10;
    }

    private void MoveTodaysInteractionEffectsToThePast()
    {
        PastInteractionEffects.AddRange(
            TodaysInteractionEffects.Select(effect =>
            {
                effect.value /= 4;
                return effect;
            })
        );

        TodaysInteractionEffects.Clear();
    }

    private void Update()
    {
        //TODO: make this run only when we get new info on someone
        // CalculateOnlyMyRelationshipWithEveryone();
    }


    // Calculate relationships with all characters based on tags and views
    public void RecalculateMyRelationshipWithEveryone()
    {
        // Accessing _character's PersonKnowledge
        var personKnowledge = _character.PersonKnowledge;
        var views = _character.Views;
        BasicFunctions.Log("Reached relationship calculations", LogType.social);

        // Get the dictionary of people's views (relationships towards _character)
        var myViews = views.PeopleViews[_character];

        // Access the _character's own knowledge of other characters
        var peopleKnowledge = personKnowledge.GetKnowledgeOf(_character);


        foreach (var characterTagsPair in peopleKnowledge)
        {
            var character = characterTagsPair.Key; // The character known by _character
            var tags = characterTagsPair.Value; // Tags (memories) that _character knows about this character




            if (myViews == null || myViews.Count == 0)
            {
                continue; // Skip if no views exist for this character
            }

            // Initialize relationship value for this character
            float relationshipValue = 0f;
            BasicFunctions.Log($"Character views contains {myViews.Count} entries", LogType.social);

            // Iterate over the tags known about the character in _character's PersonKnowledge
            foreach (var memoryTag in tags)
            {
                // Find the view for this tag in the character's views
                var viewFeelingPair = myViews.FirstOrDefault(view => view.memoryTags == memoryTag);

                if (viewFeelingPair != null)
                {
                    BasicFunctions.Log($"Character view of {memoryTag} is {viewFeelingPair.view}", LogType.social);

                    // Increase or decrease the relationship value based on the view towards the tag
                    relationshipValue += (int)viewFeelingPair.view;
                }
            }

            foreach (var effect in TodaysInteractionEffects)
            {
                relationshipValue += effect.value;
            }
            // Update the relationship value with the character based on the views and memory tags
            AddOrUpdateRelationship(_character, character, relationshipValue);

        }

    }

    // Add or update the relationship of one character towards another
    public void AddOrUpdateRelationship(Character fromCharacter, Character toCharacter, float relationshipValue)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == fromCharacter);

        if (existingPair == null)
        {
            _characterRelationships.Add(new CharacterRelationshipFloatPair
            {
                character = fromCharacter,
                relationships = new List<CharacterFloatPair>
                {
                    new CharacterFloatPair { targetCharacter = toCharacter, relationshipValue = relationshipValue }
                }
            });
        }
        else
        {
            var targetPair = existingPair.relationships.FirstOrDefault(r => r.targetCharacter == toCharacter);
            if (targetPair == null)
            {
                existingPair.relationships.Add(new CharacterFloatPair
                {
                    targetCharacter = toCharacter,
                    relationshipValue = relationshipValue
                });
            }
            else
            {
                targetPair.relationshipValue = relationshipValue;
            }
        }
    }

    // Get the relationship value between two characters
    public float GetRelationshipWith(Character whosRelationships, Character withChatacter)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == whosRelationships);

        if (existingPair != null)
        {
            var targetPair = existingPair.relationships.FirstOrDefault(r => r.targetCharacter == withChatacter);
            if (targetPair != null)
            {
                return targetPair.relationshipValue;
            }
        }

        // Default to 0 if no relationship exists
        return 0f;
    }

    // Get a list of characters that a given character has positive relationships with
    public List<Character> GetPositiveRelationships(Character character, float threshold = 0f)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == character);

        if (existingPair != null)
        {
            return existingPair.relationships
                .Where(r => r.relationshipValue > threshold)
                .Select(r => r.targetCharacter)
                .ToList();
        }

        return new List<Character>();
    }

    // Get a list of characters that a given character has negative relationships with
    public List<Character> GetNegativeRelationships(Character character, float threshold = 0f)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == character);

        if (existingPair != null)
        {
            return existingPair.relationships
                .Where(r => r.relationshipValue < threshold)
                .Select(r => r.targetCharacter)
                .ToList();
        }

        return new List<Character>();
    }
    public Character GetHighestRelationship(Character whosRelationships)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == whosRelationships);

        if (existingPair != null && existingPair.relationships.Any())
        {
            return existingPair.relationships
                .OrderByDescending(r => r.relationshipValue)
                .FirstOrDefault()?.targetCharacter;
        }

        return null; // Return null if no relationships exist
    }
    //TODO: add interface to ensure that this class and others like it has their cleanup method so we can clean up all these classes
    public void Cleanup()
    {
        WorldManager.Instance.OnMidnight -= MoveTodaysInteractionEffectsToThePast;
    }
}
