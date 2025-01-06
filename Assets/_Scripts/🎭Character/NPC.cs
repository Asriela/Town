using Mind;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Thinking))]

[RequireComponent(typeof(Senses))]



public class NPC : Character
{

    public Thinking Thinking { get; set; }
    public Senses Senses { get; set; }



    private void Start()
    {

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
