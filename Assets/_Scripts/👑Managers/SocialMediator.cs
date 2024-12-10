using UnityEngine;

public static class SocialMediator
{
    public static void PostAction(NPC sender, NPC target, string action, object actionData, float radius)
    {
        // Notify the target directly
        target?.ReactToAction(sender, action, actionData);

        // Notify nearby witnesses
        foreach (var npc in _registeredNPCs)
        {
            if (npc == sender || npc == target)
                continue; // Skip sender and direct target

            if (Vector3.Distance(sender.transform.position, npc.transform.position) <= radius)
            {
                npc.OverhearAction(sender, action, actionData);
            }
        }
    }
}
