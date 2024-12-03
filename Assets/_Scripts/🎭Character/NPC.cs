using Mind;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Thinking))]
[RequireComponent(typeof(Acting))]
[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(Senses))]
public class NPC : Character
{
    private Transform _target;

    private Thinking _thinking;
    private Senses _senses;
    private Vitality _vitality;
    public Acting Acting { get; set; }



    private void Start()
    {
        base.Start();
        _thinking = GetComponent<Thinking>();
        _thinking.Initialize(this);
        _vitality = GetComponent<Vitality>();
        _vitality.Initialize(this);
        _senses= GetComponent<Senses>();
        _senses.Initialize(this);
        Acting = GetComponent<Acting>();
        Acting.Initialize(this);
    }

    private void Update()
    {
        //thinking.Think();
        //acting.Act();

    }
}
