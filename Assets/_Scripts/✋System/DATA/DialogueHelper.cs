using System.Text.RegularExpressions;
using Mind;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
public static class DialogueHelper
{


    public static string GetTellDialogue(MemoryTags memoryTag, Character speaker, Character knower, Character aboutWho, bool aboutWhoIsPlayer)
    => memoryTag switch
    {
        MemoryTags.countryside => "I grew up in the countryside",
        MemoryTags.TheCauteris => "I'm an assurance agent sent by the Cauteris to this town.",
        MemoryTags.smallTown => "I'm from a small town.",
        MemoryTags.traveler => "I'm just a traveler..",
        MemoryTags.occult => "I was trained in the arts of the occult",
        MemoryTags.mage => "I am a mage.",
        MemoryTags.charming => "He is charming.",
        _ => GenerateResponse(memoryTag, speaker, knower, aboutWho, aboutWhoIsPlayer)
    };

    private static string GenerateResponse(MemoryTags memoryTag, Character speaker, Character knower, Character aboutWho, bool aboutWhoIsPlayer)
    {
        var tagAsString = FormatEnumName(memoryTag.ToString());
        var output = "";
        var knowerName = knower.CharacterName;
        var aboutWhoName = aboutWho.CharacterName;
        if (aboutWhoIsPlayer)
        {
            if (knower == WorldManager.Instance.ThePlayer)
            { output = $"I am {tagAsString}"; }
            else
            { output = $"{knowerName} said you are {tagAsString}"; }
        }
        else
        {
            if (speaker == knower)
            { output = $"{aboutWhoName} is {tagAsString}"; }
            else
            { output = $"{knowerName} said {aboutWhoName} is {tagAsString}"; }
        }
        return output;
    }
    private static string FormatEnumName(string enumName)
    {
        // Convert camel case to sentence case
        var formattedName = Regex.Replace(enumName, "([a-z])([A-Z])", "$1 $2");  // Adds a space between camelCase parts
                                                                                 // Capitalize the first letter of each word
        formattedName = string.Join(" ", formattedName.Split(' ').Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
        return formattedName;
    }

}
