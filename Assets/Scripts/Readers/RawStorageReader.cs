using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class RawStorageReader : MonoBehaviour
{
    [SerializeField] private EntityStorageSignals entitySignals;
    [SerializeField] private RawContentSignals rawContentSignals;
    [SerializeField] ColumnNames _individualColumnNames;
    [SerializeField] ColumnNames _familyColumnNames;

    private string _readText = "";
    private string[] _indi_lists;


    private Dictionary<string,string> _temp = new Dictionary<string, string>();

    private void Awake()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        rawContentSignals.onReadFileCompleted += OnReadFileCompleted;
    }

    private void OnReadFileCompleted()
    {
        ReadIndies();
        ReadFamilies();
    }

    private void ReadIndies()
    {
        _readText = rawContentSignals.onGetContent();
        _indi_lists = _readText.Split("INDI");

        for (int i = 0; i < _indi_lists.Length; i++)
        {
            _temp = new Dictionary<string, string>();

            if (i != 0)
            {
                _temp["Id"] = _indi_lists[i - 1].Substring(_indi_lists[i - 1].LastIndexOf("@") - 2, 2);
            }

            for (int indeks = 0; indeks < _individualColumnNames.Names.Count; indeks++)
            {
                if (_indi_lists[i].Contains(_individualColumnNames.Names[indeks]))
                {
                    _temp[_individualColumnNames.Names[indeks]] = _indi_lists[i].Split(_individualColumnNames.Names[indeks])[1].Split("\n")[0];
                }
            }

            if (_temp.Count >= 3)
            {
                SaveIndie(_temp);
            }
        }

        entitySignals.onIndieProcessCompleted?.Invoke();
    }

    private void ReadFamilies()
    {
        _readText = rawContentSignals.onGetContent();
        _indi_lists = _readText.Split("FAM");

        for (int i = 0; i < _indi_lists.Length; i++)
        {
            _temp = new Dictionary<string, string>();

            if (i != 0)
            {
                _temp["Id"] = _indi_lists[i - 1].Substring(_indi_lists[i - 1].LastIndexOf("@") - 2, 2);
            }

            for (int indeks = 0; indeks < _familyColumnNames.Names.Count; indeks++)
            {
                if (_indi_lists[i].Contains(_familyColumnNames.Names[indeks]))
                {
                    _temp[_familyColumnNames.Names[indeks]] = _indi_lists[i].Split(_familyColumnNames.Names[indeks])[1].Split("\n")[0];
                }
            }

            if (_temp.Count >= 3)
            {
                SaveFamily(_temp);
            }
        }

        entitySignals.onFamilyProcessCompleted?.Invoke();
    }

    private void SaveIndie(Dictionary<string,string> dic)
    {
        Individual tempIndi = new Individual();

        tempIndi.Name = dic["NAME"];
        tempIndi.Gender = dic["SEX"];
        tempIndi.Id = dic["Id"];

        if (dic.ContainsKey("FAMS"))
        {
            tempIndi.OwnFamily = dic["FAMS"];

        }
        if (dic.ContainsKey("FAMC"))
        {
            tempIndi.RelatedFamily = dic["FAMC"];

        }
        entitySignals.onGetIndividuals()[tempIndi.Id] = tempIndi;
    }

    private void SaveFamily(Dictionary<string, string> dic)
    {
        Family tempIndi = new Family();

        tempIndi.Husband = dic["HUSB"];
        tempIndi.Wife = dic["WIFE"];
        tempIndi.Id = dic["Id"];

        if (dic.ContainsKey("CHIL"))
        {
            tempIndi.Childs.Add(dic["CHIL"]);

        }

        entitySignals.onGetFamilies()[tempIndi.Id] = tempIndi;
    }
}