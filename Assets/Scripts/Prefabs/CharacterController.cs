using System;
using TMPro;
using UnityEngine;

class CharacterController : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;
    private Individual _myInfos;

    public void SetData(Individual individual)
    {
        _myInfos = individual;

        SetInfos();
    }

    private void SetInfos()
    {
        nameText.text = _myInfos.Name;
    }
}