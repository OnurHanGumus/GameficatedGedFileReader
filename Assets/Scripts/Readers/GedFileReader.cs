using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;
using UnityEngine;

class GEDFileReader : MonoBehaviour
{
    [SerializeField] private EntityStorageSignals storageSignals;

    public void ReadGEDFile(string filePath)
    {
        storageSignals.onGetFamilies().Clear();
        storageSignals.onGetIndividuals().Clear();

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
                        if (storageSignals.onGetIndividuals()[currentId].FAMS == null)
                        {
                            storageSignals.onGetIndividuals()[currentId].FAMS = new List<string>();
                        }
                        storageSignals.onGetIndividuals()[currentId].FAMS.Add(value);
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
                        else if (storageSignals.onGetIndividuals()[value].Gender == "F" && storageSignals.onGetIndividuals()[value].FAMS.Count == 0)
                        {
                            storageSignals.onGetFamilies()[currentId].ChildrenToShow.Add(value);
                        }
                    }
                }
            }
        }

        storageSignals.onFileOpened?.Invoke();
    }

    void Start()
    {
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("GED",".ged"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".ged");

        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
    }

    public void OpenFileExplorer()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, true, null, null, "Select Files", "Load");

        if (FileBrowser.Success)
        {
            OnFilesSelected(FileBrowser.Result);
        }
    }

    void OnFilesSelected(string[] filePaths)
    {
        // Print paths of the selected files
        for (int i = 0; i < filePaths.Length; i++)
            Debug.Log(filePaths[i]);

        string filePath = filePaths[0];
        byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(filePath);

        string destinationPath = Path.Combine(Application.persistentDataPath, FileBrowserHelpers.GetFilename(filePath));
        FileBrowserHelpers.CopyFile(filePath, destinationPath);

        ReadGEDFile(filePath);
    }
}
