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
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI scoreText;

    Dictionary<string, Family> families;
    Dictionary<string, Individual> individuals;
    Individual indi;
    Individual answerIndi;

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
        scoreText.text = 0.ToString();
    }

    public void AskQuestionButton()
    {
        AskQuestion();
    }

    private void AskQuestion()
    {
        do
        {
            int indiIndeks = Random.Range(0, individuals.Count);
            indi = individuals.ElementAt(indiIndeks).Value;

            answerIndi = FindCouple(indi);

        } while (answerIndi == null);

        questionText.text = "What is the name of " + indi.Name + "'s partner?";
        ArrangeAnswers(answerIndi);
    }

    private void ArrangeAnswers(Individual answerIndi)
    {
        foreach (var i in choiceTexts)
        {
            Individual wrongIndi = individuals.ElementAt(Random.Range(0, individuals.Count)).Value;
            if (wrongIndi.Id != answerIndi.Id)
            {
                i.text = wrongIndi.Name;
            }
        }

        choiceTexts[Random.Range(0, choiceTexts.Length)].text = answerIndi.Name;
    }

    private Individual FindCouple(Individual indi)
    {
        try
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
        catch (System.Exception)
        {

            return null;
        }
        
    }

    public void AnswerButton(int id)
    {
        if (choiceTexts[id].text == answerIndi.Name)
        {
            scoreText.text = (int.Parse(scoreText.text) + 10).ToString();
        }

        AskQuestion();
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
