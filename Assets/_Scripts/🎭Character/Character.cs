using Mind;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(Appearance))]
[RequireComponent(typeof(Memory))]
[RequireComponent(typeof(UI))]
[RequireComponent(typeof(Reactions))]
[RequireComponent(typeof(State))]
[RequireComponent(typeof(PersonKnowledge))]
[RequireComponent(typeof(Views))]
[RequireComponent(typeof(Relationships))]
[RequireComponent(typeof(VisualStatusKnowledge))]
public class Character : MonoBehaviour
{
    private Collider2D _collider2D;

    [SerializeField]
    private CharacterName _characterName;



    public CharacterName CharacterName => _characterName;

    public UI Ui { get; set; }
    public Movement Movement { get; set; }
    public Vitality Vitality { get; set; }

    public Memory Memory { get; set; }

    public Reactions Reactions { get; set; }

    public Appearance Appearance { get; set; }
    public State State { get; set; }

    public PersonKnowledge PersonKnowledge { get; set; }
    public Views Views { get; set; }

    public Relationships Relationships { get; set; }
    public VisualStatusKnowledge VisualStatusKnowledge { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    private void Awake()
    { 
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        Movement = GetComponent<Movement>();
        Vitality = GetComponent<Vitality>();
        Vitality.Initialize(this);
        Appearance = GetComponent<Appearance>();
        Appearance.Initialize(this);
        PersonKnowledge = GetComponent<PersonKnowledge>();
        Views = GetComponent<Views>();

        Memory = GetComponent<Memory>();
        Ui = GetComponent<UI>();
        Ui.Initialize(this);
        Reactions = GetComponent<Reactions>();
        Reactions.Initialize(this);
        State = GetComponent<State>();
        State.Initialize(this);
        Relationships = GetComponent<Relationships>();
        Relationships.Initialize(this);
        VisualStatusKnowledge = GetComponent<VisualStatusKnowledge>();

    }


    public Collider2D GetCollider2D() => _collider2D;

}
