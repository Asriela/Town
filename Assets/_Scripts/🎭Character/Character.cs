using Mind;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(Appearance))]
public class Character : MonoBehaviour
{
    private Collider2D _collider2D;
    public Movement Movement { get; set; }
    public Vitality Vitality { get; set; }

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
    }


    public Collider2D GetCollider2D() => _collider2D;

}
