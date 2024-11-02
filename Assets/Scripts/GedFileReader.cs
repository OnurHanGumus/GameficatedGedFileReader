using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GedFileReader : MonoBehaviour
{
    private const string _file_location = "C:\\Users\\onurh\\OneDrive\\Documents\\GameficatedGedFileReader\\";
    private string _file_name = "MyGed.ged";

    private void Awake()
    {
        ReadFile();
    }

    private void ReadFile()
    {
        String line;
        try
        {
            //Pass the file path and file name to the StreamReader constructor
            StreamReader sr = new StreamReader(_file_location + _file_name);
            line = sr.ReadLine();
            while (line != null)
            {
                line = sr.ReadLine();
                Debug.Log(line);
            }

            sr.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            Debug.LogWarning("File read process was successful");

        }
    }
}
