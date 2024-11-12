using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RawContentStorage : MonoBehaviour
{
    [SerializeField] private RawContentSignals rawContentSignals;
    private string _content = "";

    private void Awake()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        rawContentSignals.onGetContent += OnGetContent;
        rawContentSignals.onSetContent += OnSetContent;
    }

    public string OnGetContent()
    {
        return _content;
    }

    public void OnSetContent(string content)
    {
        _content = content;
    }
}