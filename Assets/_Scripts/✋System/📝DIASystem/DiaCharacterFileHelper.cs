using UnityEngine;

public static class DiaCharacterFileHelper
{
    public static string GetFileName(Character character)
    => character.CharacterName switch
    {

        Mind.CharacterName.Alrine => GetAlrineFile(),
        _ => null
    };

    private static string GetAlrineFile()
    {
        var ret = "Alrine_welcome";

        return ret;
    }

}
