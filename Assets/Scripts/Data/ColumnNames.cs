using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColumnNames", menuName = "ScriptableObjects/ColumnNames", order = 1)]
public class ColumnNames : ScriptableObject
{
    public List<string> Names = new List<string>();

}