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
    [SerializeField] private List<string> _childs;

    private string _readText = "";
    private string[] _indi_lists;


    private Dictionary<string,string> _tempRecord = new Dictionary<string, string>();

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
            _tempRecord = new Dictionary<string, string>();
            GetId(i);

            for (int indeks = 0; indeks < _individualColumnNames.Names.Count; indeks++)
            {
                if (_indi_lists[i].Contains(_individualColumnNames.Names[indeks]))
                {
                    _tempRecord[_individualColumnNames.Names[indeks]] = _indi_lists[i].Split(_individualColumnNames.Names[indeks])[1].Split("\n")[0];
                }
            }

            if (_tempRecord.Count >= 3)
            {
                SaveIndie(_tempRecord);
            }
        }

        entitySignals.onIndieProcessCompleted?.Invoke();
    }

    private void GetId(int indi_list_indeks)
    {
        if (indi_list_indeks != 0)
        {
            bool isNum = true;
            int counter = 1;
            while (isNum == true)
            {
                isNum = int.TryParse(_indi_lists[indi_list_indeks - 1].Substring(_indi_lists[indi_list_indeks - 1].LastIndexOf("@") - counter, 1), out int a);
                ++counter;
            }
            _tempRecord["Id"] = _indi_lists[indi_list_indeks - 1].Substring(_indi_lists[indi_list_indeks - 1].LastIndexOf("@") - counter + 1, counter - 1);
        }
    }

    private void ReadFamilies()
    {
        _readText = rawContentSignals.onGetContent();
        _indi_lists = _readText.Split("FAM");

        for (int indi_list_indeks = 0; indi_list_indeks < _indi_lists.Length; indi_list_indeks++)
        {
            _tempRecord = new Dictionary<string, string>();

            GetId(indi_list_indeks);

            for (int column_indeks = 0; column_indeks < _familyColumnNames.Names.Count; column_indeks++)
            {
                if (_indi_lists[indi_list_indeks].Contains(_familyColumnNames.Names[column_indeks]))
                {
                    _tempRecord[_familyColumnNames.Names[column_indeks]] = _indi_lists[indi_list_indeks].Split(_familyColumnNames.Names[column_indeks])[1].Split("\n")[0];
                    //Debug.Log(_indi_lists[indi_list_indeks].Split(_familyColumnNames.Names[column_indeks])[1].Split("\n")[0]);
                    //_childs.Add();
                }
            }

            if (_tempRecord.Count >= 3)
            {
                SaveFamily(_tempRecord);
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

        if (dic.ContainsKey("HUSB"))
        {
            tempIndi.Husband = dic["HUSB"];

        }
        if (dic.ContainsKey("WIFE"))
        {
            tempIndi.Wife = dic["WIFE"];

        }

        tempIndi.Id = dic["Id"];

        if (dic.ContainsKey("CHIL"))
        {
            tempIndi.Childs.Add(dic["CHIL"]);

        }

        entitySignals.onGetFamilies()[tempIndi.Id] = tempIndi;
    }
}