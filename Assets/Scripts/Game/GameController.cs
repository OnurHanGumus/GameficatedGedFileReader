using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private Button openGameButton;
    [SerializeField] private EntityStorageSignals signals;
    [SerializeField] private TextMeshProUGUI[] choiceTexts;

    Dictionary<string, Family> families;
    Dictionary<string, Individual> individuals;

    private void Start()
    {
        SubscribeEvents();
        Init();
    }

    private void Init()
    {
        families = signals.onGetFamilies();
        individuals = signals.onGetIndividuals();
    }

    private void SubscribeEvents()
    {
        signals.onFileOpened += OnFileOpened;
    }

    private void OnFileOpened()
    {
        openGameButton.interactable = true;
    }

    private void AskQuestion()
    {
        int indiIndeks = Random.Range(0, individuals.Count);
        Individual indi = individuals.ElementAt(indiIndeks).Value;

        Individual answerIndi = FindCouple(indi);
        while (answerIndi == null)
        {
            answerIndi = FindCouple(indi);
        }
        
    }

    private Individual FindCouple(Individual indi)
    {
        if (indi.FAMS.Count < 0)
        {
            return null;
        }
        if (indi.Gender == "M")
        {
            return individuals[families[indi.FAMS[0]].Wife];
        }
        else if (indi.Gender == "F")
        {
            return individuals[families[indi.FAMS[0]].Husband];
        }
        else
        {
            return null;
        }
    }

    private Individual FindFatherName(Individual indi)
    {

        foreach (var i in families.Values)
        {
            foreach (var y in i.Children)
            {
                if (y == indi.Id)
                {
                    return individuals[y];
                }
            }
        }

        return null;
    }
}
