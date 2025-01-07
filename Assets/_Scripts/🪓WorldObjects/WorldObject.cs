using System;
using System.Collections.Generic;
using Mind;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.Examples.TMP_ExampleScript_01;
//TODO: change this object into a parent child string of objects to devide up the object types


public class WorldObject : MonoBehaviour
{
    [SerializeField]
    private ObjectType _objectType;
    public ObjectType ObjectType => _objectType;

    [SerializeField]
    private float _integrity;
    public float Integrity => _integrity;

    public float RentHoursLeft { get; set; } = 0;

    public Character RentedBy { get; set; } = null;
    public Location RentedFrom { get; set; } = null;

    private List<InteractionOption> interactionOptions  = new List<InteractionOption>();
    public class InteractionOption
    {
        public string Label { get; }
        public Action InteractionAction { get; }

        public InteractionOption(string label, Action interactionAction)
        {
            Label = label;
            InteractionAction = interactionAction;
        }

        public void Execute()
        {
            InteractionAction?.Invoke();
        }
    }

    [SerializeField]
    private float _care;
    public float Care => _care;

    [SerializeField]
    private float _size;
    public float Size => _size;

    private List<ObjectTrait> _objectTraits = new();
    private SpriteRenderer _spriteRenderer;
    private void Start()
    {
        _integrity = 100;
        _care = 50;
        _size = 1;
        SetStartingTraitsBasedOnObjectType();

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }



    public void SetVisibility(bool visible)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = visible; // Hides the sprite
        }
    }

    private void Update()
    {

        Grow();
        Wilt();
        TimelapseRent();
    }

    public void StartRenting(Character character , Location location, float hours)
    {
        RentedFrom = location;
        RentedBy = character;
        RentHoursLeft = hours;
    }
    private void TimelapseRent()
    {
        if (RentedBy == null)
        {
            RentHoursLeft = 0;
            return;
        }


        RentHoursLeft -= WorldManager.Instance.TimeThatsChanged;
        if (RentHoursLeft <= 0)
        {
            RentedBy.Memory.RemoveObjectFromPossessions(this);
            RentedFrom.AddPosession(_objectType, this);
            RentedBy = null;
        }
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

    private void Use()
    {
        switch (_objectType)
        {
            case ObjectType.bed:

                break;
        }
    }

    public List<InteractionOption> GetInteractionOptions(Character character)
    {
        interactionOptions.Clear();
        switch (_objectType)
        {
            case ObjectType.bed:
                if(character.Memory.IsOurPossession(this))
                { interactionOptions.Add(new InteractionOption("Sleep", () => Use()));}

                break;
        }

        return interactionOptions;
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
    public void Use(Character userOfObject, out bool destroy)
    {
        destroy = false;
        switch (_objectType)
        {
            //TODO: should just active its effect which is defined in the world object class as opposed to doing a switch
            case ObjectType.bookOfTheDead:
                userOfObject.Memory.AddTrait(GameManager.Instance.TraitsInPlay[TraitType.deathCultist]);
                break;
            case ObjectType.ale:
                _integrity -= 0.1f;
                if (_integrity <= 0)
                { destroy = true; }

                break;
        }


    }

    public void CareFor(Character userOfObject)
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
