using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class RawContentSignals : MonoBehaviour
{
    public Func<string> onGetContent { get; set; }
    public UnityAction<string> onSetContent = delegate { };
    public UnityAction onReadFileCompleted = delegate { };
}