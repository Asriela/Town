using Mind;
using UnityEngine;

public class UseFeature : MonoBehaviour, IUseable
{
    private readonly WorldObject _worldObject;

    public UseFeature(WorldObject worldObject) => _worldObject = worldObject;

    public void Use(Character userOfObject)
    {
        var destroy = false;
        switch (_worldObject.ObjectType)
        {
            case ObjectType.bookOfTheDead:
                userOfObject.Memory.AddTrait(GameManager.Instance.TraitsInPlay[TraitType.deathCultist]);
                break;
            case ObjectType.ale:
                _worldObject.Integrity -= 0.1f;
                if (_worldObject.Integrity <= 0)
                {
                    destroy = true;
                }
                break;
            case ObjectType.bed:
                userOfObject.State.SetState(StateType.sleeping);
                break;
        }

        if (destroy)
        {
            ActionsHelper.DestroyObject(userOfObject, _worldObject);
        }
    }
}
