using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

class GEDFileReader : MonoBehaviour
{
    [SerializeField] private EntityStorageSignals storageSignals;
    private const string _file_location = "C:\\Users\\onurh\\OneDrive\\Documents\\GameficatedGedFileReader\\";
    private string _file_name = "Harry.ged";

    private void Start()
    {
        ReadGEDFile(_file_location + _file_name);
        //PrintData();
    }

    public void ReadGEDFile(string filePath)
    {
        string currentId = null;
        string currentType = null;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2) continue;

                string level = parts[0];
                string tag = parts[1];
                string value = parts.Length > 2 ? parts[2] : "";

                if (level == "0")
                {
                    if (tag.StartsWith("@") && value == "INDI")
                    {
                        currentId = tag;
                        currentType = "INDI";
                        storageSignals.onGetIndividuals()[currentId] = new Individual { Id = currentId };
                    }
                    else if (tag.StartsWith("@") && value == "FAM")
                    {
                        currentId = tag;
                        currentType = "FAM";
                        storageSignals.onGetFamilies()[currentId] = new Family { Id = currentId };
                    }
                    else
                    {
                        currentType = null;
                    }
                }
                else if (currentType == "INDI")
                {
                    if (tag == "NAME")
                    {
                        var nameParts = value.Split('/');
                        storageSignals.onGetIndividuals()[currentId].Name = nameParts[0].Trim();
                        storageSignals.onGetIndividuals()[currentId].Surname = nameParts.Length > 1 ? nameParts[1].Trim() : "";
                    }
                    else if (tag == "SEX")
                    {
                        storageSignals.onGetIndividuals()[currentId].Gender = value;
                    }
                    else if (tag == "FAMC")
                    {
                        storageSignals.onGetIndividuals()[currentId].FAMC = value;
                    }
                    else if (tag == "FAMS")
                    {
                        storageSignals.onGetIndividuals()[currentId].FAMS = value;
                    }
                }
                else if (currentType == "FAM")
                {
                    if (tag == "HUSB")
                    {
                        storageSignals.onGetFamilies()[currentId].Husband = value;
                    }
                    else if (tag == "WIFE")
                    {
                        storageSignals.onGetFamilies()[currentId].Wife = value;
                    }
                    else if (tag == "CHIL")
                    {
                        storageSignals.onGetFamilies()[currentId].Children.Add(value);
                        if (storageSignals.onGetIndividuals()[value].Gender == "M")
                        {
                            storageSignals.onGetFamilies()[currentId].ChildrenToShow.Add(value);
                        }
                        else if (storageSignals.onGetIndividuals()[value].Gender == "F" && storageSignals.onGetIndividuals()[value].FAMS == null)
                        {
                            storageSignals.onGetFamilies()[currentId].ChildrenToShow.Add(value);
                        }
                    }
                }
            }
        }
    }

    public void PrintData()
    {
        Debug.Log("Individuals");
        foreach (var individual in storageSignals.onGetIndividuals().Values)
        {
            Debug.Log($"ID: {individual.Id}, Name: {individual.Name}, Surname: {individual.Surname}, Gender: {individual.Gender}, FAMC: {individual.FAMC}, FAMS: {individual.FAMS}");
        }

        Debug.Log("Individuals");
        foreach (var family in storageSignals.onGetFamilies().Values)
        {
            Debug.Log($"ID: {family.Id}, Husband: {family.Husband}, Wife: {family.Wife}, Children: {string.Join(", ", family.Children)}");
        }
    }
}
