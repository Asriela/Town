using System;
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
    [SerializeField]
    private List<CharacterRelationshipFloatPair> _characterRelationships = new();

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
