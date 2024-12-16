using System.Collections.Generic;
using Mind;
using Unity.VisualScripting;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [SerializeField]
    private ObjectType _objectType;
    public ObjectType ObjectType => _objectType;

    [SerializeField]
    private float _integrity;
    public float Integrity => _integrity;

    [SerializeField]
    private float _care;
    public float Care => _care;

    [SerializeField]
    private float _size;
    public float Size => _size;

    private List<ObjectTrait> _objectTraits = new();

    private void Start()
    {
        _integrity = 100;
        _care = 50;
        _size = 1;
        SetStartingTraitsBasedOnObjectType();
    }

    private void Update()
    {

        Grow();
        Wilt();

    }

    private void Wilt()
    {
        if (!_objectTraits.Contains(ObjectTrait.wilts))
        { return; }

        if (_care < 10)
        {
            _integrity -= 1;
        }
    }

    private void Grow()
    {
        if (!_objectTraits.Contains(ObjectTrait.grows))
        { return; }
        if (_care > 40)
        {
            _integrity += 1;
            _size += 1;
        }


    }

    private void SetStartingTraitsBasedOnObjectType()
    {
        switch (_objectType)
        {
            case ObjectType.pumpkin:
                _objectTraits.Add(ObjectTrait.crop);
                _objectTraits.Add(ObjectTrait.wilts);
                _objectTraits.Add(ObjectTrait.grows);
                _objectTraits.Add(ObjectTrait.plant);
                break;
        }
    }
    public Vector3 GetPosition() => transform.position;
    //TODO: make that character class holds traits so that maybe player also has traits that limmit what the player can do or how they react
    public void Use(NPC userOfObject)
    {
        switch (_objectType)
        {
            //TODO: should just active its effect which is defined in the world object class as opposed to doing a switch
            case ObjectType.bookOfTheDead:
                userOfObject.Thinking.AddTrait(GameManager.Instance.TraitsInPlay[TraitType.murderer]);
                break;
        }
    }

    public void CareFor(NPC userOfObject)
    {
        foreach (var trait in _objectTraits)
        {
            switch (trait)
            {
                case ObjectTrait.crop:
                    _care += 100;
                    break;
            }
        }
    }
}
