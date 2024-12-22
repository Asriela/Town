using UnityEngine;
using UnityEngine.TextCore.Text;

public static class SocialMediator
{
    private static readonly float _hearingRadius = 2f;
    private static readonly float _sightRadius = 10f;

    public static void PostAction(Character sender, Character target, Mind.ActionType actionType, ActionPost actionPost)
    {
        // Notify the target directly
        if (target != null)
        { target.Reactions.ReactToActionStarter(sender, true, actionType, actionPost); }

        if (IsHeardAction(actionType))
        {
            foreach (var characterKeyValue in WorldManager.Instance.AllCharacters)
            {
                var character = characterKeyValue.Value;
                if (character == sender || character == target)
                { continue; }

                //TODO: change hearing radius to senses : add senses to character class instead of npc class
                if (Vector3.Distance(sender.transform.position, character.transform.position) <= _hearingRadius)
                {
                    character.Reactions.ReactToActionStarter(sender, false, actionType, actionPost);
                }
            }
        }

        if (IsSeenAction(actionType))
        {
            foreach (var characterKeyValue in WorldManager.Instance.AllCharacters)
            {
                var character = characterKeyValue.Value;
                if (character == sender || character == target)
                { continue; }

                //TODO: change sightradius to checking line of sight via senses class
                if (Vector3.Distance(sender.transform.position, character.transform.position) <= _sightRadius)
                {
                    character.Reactions.ReactToActionStarter(sender, false, actionType, actionPost);
                }
            }
        }

    }

    private static bool IsHeardAction(Mind.ActionType actionType)
    {
        bool ret = false;
        switch (actionType)
        {
            case Mind.ActionType.findKnowledge:
                ret = true;
                break;
            case Mind.ActionType.socialize:
                ret = true;
                break;
        }
        return ret;
    }

    private static bool IsSeenAction(Mind.ActionType actionType)
    {
        bool ret = false;
        switch (actionType)
        {
            case Mind.ActionType.kill:
                ret = true;
                break;
            case Mind.ActionType.performSpell:
                ret = true;
                break;
        }
        return ret;
    }

}



