using Mind;
using UnityEngine;

public class EffectsFeature : MonoBehaviour, IEffetable
{
    private WorldObject _worldObject;

    public void Setup(WorldObject worldObject) => _worldObject = worldObject;

    private void Update()
    {
        EffectWearer(_worldObject.WhoImInPossesionOf);
    }
    public void EffectWearer(Character wearer)
    {
        var destroy = false;
        switch (_worldObject.ObjectType)
        {
            case ObjectType.cursedWerewolfAmulet:
                //TODO: add that player has senses so they can also see and use this feature
                if (wearer is NPC)
                {
                    var npc = wearer as NPC;
                    if (npc.Senses.SeeSomeoneWithTraits(TraitType.captainOfTheGuard))
                    {
                        wearer.State.SetFormState(MemoryTags.werewolf);
                        wearer.Appearance.SetSprite("Werewolf");
                        wearer.Memory.AddTrait(GameManager.Instance.TraitsInPlay[TraitType.werewolf]);
                    }
                }

                break;
            case ObjectType.ale:
                _worldObject.Integrity -= 0.1f;
                if (_worldObject.Integrity <= 0)
                {
                    destroy = true;
                }
                break;

        }

        if (destroy)
        {
            ActionsHelper.DestroyObject(wearer, _worldObject);
        }
    }
}
