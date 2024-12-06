using Mind;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Thinking))]
[RequireComponent(typeof(Acting))]
[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(Senses))]
[RequireComponent(typeof(Logger))]
[RequireComponent(typeof(Memory))]
public class NPC : Character
{
    private Transform _target;

    public Thinking Thinking{ get; set; }

    public Vitality Vitality { get; set; }
    public Memory Memory { get; set; }
    public Senses Senses { get; set; }
    public Acting Acting { get; set; }

    public Logger Logger { get; set; }

    protected override void Start()
    {
        base.Start();
        Thinking = GetComponent<Thinking>();
        Thinking.Initialize(this);
        Vitality = GetComponent<Vitality>();
        Vitality.Initialize(this);
        Senses = GetComponent<Senses>();
        Senses.Initialize(this);
        Acting = GetComponent<Acting>();
        Acting.Initialize(this);
        Logger = GetComponent<Logger>();
        Memory = GetComponent<Memory>();
    }

    private void Update()
    {
        //thinking.Think();
        //acting.Act();

    }
}
