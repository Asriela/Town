using Mind;
using UnityEngine;

public static class PlayerDialogue
{
    public static string GetPlayerDialogue(MemoryTags memoryTag)
        => memoryTag switch
        {
            MemoryTags.countryside => "I grew up in the countryside",
            MemoryTags.TheCauteris => "Im an assurance agent sent by the Cauteris to this town.",
            MemoryTags.smallTown => "Im from a small town.",
            MemoryTags.traveler => "Im just a traveler..",
            MemoryTags.occult => "I was trained in the arts of the occult",
            _ => ""
        };

}
