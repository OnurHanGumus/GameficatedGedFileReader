using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Family
{
    public string Husband { get; set; } = "";
    public string Wife { get; set; } = "";
    public string Id { get; set; } = "";
    public List<string> Children { get; set; } = new List<string>();
    public List<string> ChildrenToShow { get; set; } = new List<string>();
}