using Mind;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Thinking))]

[RequireComponent(typeof(Senses))]

[RequireComponent(typeof(Acting))]



public class NPC : Character
{

    public Thinking Thinking { get; set; }
    public Senses Senses { get; set; }


    public Acting Acting { get; set; }

    private void Start()
    {
        Acting = GetComponent<Acting>();
        Acting.Initialize(this);
        Thinking = GetComponent<Thinking>();
        Thinking.Initialize(this);
        Senses = GetComponent<Senses>();
        Senses.Initialize(this);



    }

    private void Update()
    {
        //thinking.Think();
        //acting.Act();

    }
}
