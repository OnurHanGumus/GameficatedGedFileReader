using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GedFileReader : MonoBehaviour
{
    [SerializeField] private RawContentSignals rawContentSignals;

    private const string _file_location = "C:\\Users\\onurh\\OneDrive\\Documents\\GameficatedGedFileReader\\";
    private string _file_name = "MyGed.ged";

    private string _read_part = "";

    private void Start()
    {
        ReadFile();
    }

    private void ReadFile()
    {
        string line;
        StreamReader sr = new StreamReader(_file_location + _file_name);

        while ((line = sr.ReadLine()) != null)
        {
            _read_part += line + "\n";
        }

        rawContentSignals.onSetContent?.Invoke(_read_part);
        rawContentSignals.onReadFileCompleted?.Invoke();

        sr.Close();
    }
}
