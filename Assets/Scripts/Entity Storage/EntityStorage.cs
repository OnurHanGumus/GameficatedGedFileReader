using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EntityStorage : MonoBehaviour
{
    public Dictionary<string,Family> Families = new Dictionary<string, Family>();
    public Dictionary<string,Individual> Individuals = new Dictionary<string,Individual>();

    [SerializeField] private EntityStorageSignals entitySignals;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        entitySignals.onGetIndividuals += OnGetIndividuals;
        entitySignals.onGetFamilies += OnGetFamilies;
        entitySignals.onSetContent += OnSetContent;
        entitySignals.onIndieProcessCompleted += OnIndieReadProcessCompleted;
        entitySignals.onFamilyProcessCompleted += OnFamilyReadProcessCompleted;
    }

    private void OnFamilyReadProcessCompleted()
    {
        foreach (var i in Families.Values)
        {
            Debug.Log("ID: " + i.Id + "\nHusb: " + i.Husband + "\nWife: " + i.Wife + "\nChild: " + i.Childs[0]);
        }
    }

    public void OnIndieReadProcessCompleted()
    {
        foreach (var i in Individuals.Values)
        {
            Debug.Log("ID: " + i.Id + "\nName: " + i.Name + "\nGender: " + i.Gender + "\nRelated Family: " + i.RelatedFamily + "\nOwn Family: " + i.OwnFamily);
        }
    }

    public Dictionary<string,Individual> OnGetIndividuals()
    {
        return Individuals;
    }

    public Dictionary<string, Family> OnGetFamilies()
    {
        return Families;
    }

    public void OnSetContent(Dictionary<string,Individual> list)
    {
        Individuals = list;
    }
}