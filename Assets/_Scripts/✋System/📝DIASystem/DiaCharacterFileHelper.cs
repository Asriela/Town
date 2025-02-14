using Mind;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class DiaCharacterFileHelper
{
    private static Dictionary<CharacterName, int> accessCount = new();



    public static string GetFileName(Character character, DialogueFileType fileType)
    {
        string ret= string.Empty;
        var name = character.CharacterName;
        switch (fileType)
        {
            case DialogueFileType.auto:
                ret = character.CharacterName switch
                {

                    Mind.CharacterName.Agnar => GetAgnarFile(character),
                    Mind.CharacterName.Elara => GetElaraFile(character),
                    //Mind.CharacterName.Onar => GetOnarFile(character),
                    _ => Default(character)
                };
                break;
            case DialogueFileType.confront:
                ret = $"{name}_confront";
                accessCount[name]++;
                break;
        }


        return ret;
    }

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
            var relationshipValue = character.Relationships.GetRelationshipWith(character, WorldManager.Instance.ThePlayer);
            if (relationshipValue < 0)
            { ret = $"{name}_badRelationship"; }
            else
            { ret = $"{name}_repeat"; }
        }

        accessCount[name]++;
        return ret;
    }
    private static string GetOnarFile(Character character)
    {
        var name = character.CharacterName;
        var ret = $"{name}_repeat";
        return ret;
    }
    private static string GetElaraFile(Character character)
    {
        var name = character.CharacterName;
        var ret = $"{name}_1";

        if(WorldManager.Instance.Day>1)
            ret = $"{name}_2";

        accessCount[name]++;
        return ret;
    }
}
