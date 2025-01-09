using System;
using System.Collections.Generic;
using System.Linq;
using Mind;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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



public class Relationships : MonoBehaviour
{
    private Character _character;

    public void Initialize(Character character)
    {
        _character = character;
    }

    [SerializeField]
    private List<CharacterRelationshipFloatPair> _characterRelationships = new();





    private void Update()
    {
        //TODO: make this run only when we get new info on someone
        CalculateOnlyMyRelationshipWithEveryone();
    }


    // Calculate relationships with all characters based on tags and views
    public void CalculateOnlyMyRelationshipWithEveryone()
    {
        var personKnowledge = _character.GetComponent<PersonKnowledge>();
        var views = _character.GetComponent<Views>();

        var peopleKnowledge = personKnowledge.PeopleKnowledge;
        var peopleViews = views.PeopleViews;

        foreach (var characterTagsPair in peopleKnowledge)
        {
            var character = characterTagsPair.Key;
            var tags = characterTagsPair.Value;

            // Get views for the character
            var characterViews = peopleViews.ContainsKey(character) ? peopleViews[character] : new List<MemoryTagsFeelingsPair>();

            float relationshipValue = 0f;

            // Iterate over the tags known about the character
            foreach (var tag in tags)
            {
                // Find the view for this tag
                var viewFeelingPair = characterViews.FirstOrDefault(view => view.memoryTags == tag);
                if (viewFeelingPair != null)
                {
                    // Increase or decrease the relationship value based on the view towards the tag
                    relationshipValue += (int)viewFeelingPair.view;
                }
         
            }

            // Update the relationship value with the character
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
    public float GetRelationshipWith(Character fromCharacter, Character toCharacter)
    {
        var existingPair = _characterRelationships.FirstOrDefault(p => p.character == fromCharacter);

        if (existingPair != null)
        {
            var targetPair = existingPair.relationships.FirstOrDefault(r => r.targetCharacter == toCharacter);
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
}
