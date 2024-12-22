using Mind;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(Appearance))]
[RequireComponent(typeof(Reactions))]
[RequireComponent(typeof(Memory))]
[RequireComponent(typeof(UI))]
public class Character : MonoBehaviour
{
    private Collider2D _collider2D;

    [SerializeField]
    private CharacterName _characterName;



    public CharacterName CharacterName => _characterName;

    public UI Ui { get; set; }
    public Movement Movement { get; set; }
    public Vitality Vitality { get; set; }
    public Reactions Reactions { get; set; }

    public Memory Memory { get; set; }



    public Appearance Appearance { get; set; }

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
        Reactions = GetComponent<Reactions>();
        Reactions.Initialize(this);
        Memory = GetComponent<Memory>();
        Ui = GetComponent<UI>();
        Ui.Initialize(this);
    }


    public Collider2D GetCollider2D() => _collider2D;

}
