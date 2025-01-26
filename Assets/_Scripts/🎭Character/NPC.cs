using Mind;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Thinking))]

[RequireComponent(typeof(Senses))]

[RequireComponent(typeof(Actions))]



public class NPC : Character
{

    public Thinking Thinking { get; set; }
    public Senses Senses { get; set; }


    public Actions Acting { get; set; }

    private void Start()
    {
        Senses = GetComponent<Senses>();
        Senses.Initialize(this);
        Acting = GetComponent<Actions>();
        Acting.Initialize(this);
        Thinking = GetComponent<Thinking>();
        Thinking.Initialize(this);




    }

    private void Update()
    {
        //thinking.Think();
        //acting.Act();

    }
}
