using Mind;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [SerializeField]
    private Mind.ObjectType _objectType;
    public Mind.ObjectType ObjectType => _objectType;
    public Vector3 GetPosition() => transform.position;
    //TODO: make that character class holds traits so that maybe player also has traits that limmit what the player can do or how they react
    public void Use(NPC userOfObject)
    {
        switch (_objectType)
        {
            //TODO: should just active its effect which is defined in the world object class as opposed to doing a switch
            case Mind.ObjectType.bookOfTheDead:
                userOfObject.Thinking.AddTrait(GameManager.Instance.TraitsInPlay[Mind.TraitType.murderer]);
                break;
        }
    }
}
