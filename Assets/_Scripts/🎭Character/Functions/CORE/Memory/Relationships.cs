﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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
        var peopleKnowledge = personKnowledge.PeopleKnowledge;

        // Iterate over each person in _character's PersonKnowledge
        foreach (var characterTagsPair in peopleKnowledge)
        {
            var character = characterTagsPair.Key;  // The character known by _character
            var tags = characterTagsPair.Value;    // Tags (memories) that _character knows about this character




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
