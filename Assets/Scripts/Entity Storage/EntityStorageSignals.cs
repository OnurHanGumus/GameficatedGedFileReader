using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class EntityStorageSignals : MonoBehaviour
{
    public Func<Dictionary<string, Individual>> onGetIndividuals { get; set; }
    public Func<Dictionary<string, Family>> onGetFamilies { get; set; }
    public UnityAction<Dictionary<string, Individual>> onSetContent = delegate { };
    public UnityAction onIndieProcessCompleted = delegate { };
    public UnityAction onFamilyProcessCompleted = delegate { };
    public UnityAction onFileOpened = delegate { };
}