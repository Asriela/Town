using Mind;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class DiaCharacterFileHelper
{
    private static Dictionary<CharacterName, int> accessCount  = new();



    public static string GetFileName(Character character)
    => character.CharacterName switch
    {

        Mind.CharacterName.Agnar => Default(character),
        _ => Default(character)
    };

    public static void InitializeAccessCounts()
    {
        // Iterate through all CharacterName enum values and initialize them to 0
        foreach (CharacterName character in Enum.GetValues(typeof(CharacterName)))
        {
            accessCount[character] = 0;
        }
    }

    private static string Default(Character character)
    {
        var name = character.CharacterName;
        var ret = $"{name}_1";
        if (accessCount[name] > 0)
        {
             ret = $"{name}_repeat";
        }


        accessCount[name]++;
        return ret;
    }
    private static string GetAgnarFile(Character character)
    {
        var name = character.CharacterName;
        var ret = $"{name}_1";
        if (accessCount[name] > 0)
        {
            var relationshipValue= character.Relationships.GetRelationshipWith(character, WorldManager.Instance.ThePlayer);
            if (relationshipValue < 0)
            { ret = $"{name}_badRelationship"; }
            else
            { ret = $"{name}_repeat"; }
        }

        accessCount[name]++;
        return ret;
    }

}
