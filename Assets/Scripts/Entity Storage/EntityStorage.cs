using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EntityStorage : MonoBehaviour
{
    public Dictionary<string, Individual> Individuals { get; private set; } = new Dictionary<string, Individual>();
    public Dictionary<string, Family> Families { get; private set; } = new Dictionary<string, Family>();

    [SerializeField] private EntityStorageSignals entitySignals;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        entitySignals.onGetIndividuals += OnGetIndividuals;
        entitySignals.onGetFamilies += OnGetFamilies;
    }

    public Dictionary<string,Individual> OnGetIndividuals()
    {
        return Individuals;
    }

    public Dictionary<string, Family> OnGetFamilies()
    {
        return Families;
    }
}