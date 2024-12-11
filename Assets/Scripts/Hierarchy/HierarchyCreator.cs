using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HierarchyCreator : MonoBehaviour
{
    public GameObject WomanPrefab; // Prefab for individual nodes
    public GameObject ManPrefab; // Prefab for individual nodes
    public Material LineMaterial; // Material for LineRenderer
    public float Spacing = 1f; // Spacing between nodes
    public float FamilyHorizontalSpacing = 2f; // Spacing between nodes
    public float IndividualHorizontalSpacing = 1f; // Spacing between nodes
    [SerializeField] private EntityStorageSignals entityStorageSignals;
    private Dictionary<string, GameObject> createdFamilies = new Dictionary<string, GameObject>();
    private Dictionary<string, int> nodesInteractionCounts = new Dictionary<string, int>();
    private Dictionary<string, GameObject> allIndividualGameObjects = new Dictionary<string, GameObject>();
    private int maxConnectionCount = 0;
    public void Start()
    {
        Invoke("CreateHierarchy", 1f);
    }

    public void CreateHierarchy()
    {
        // Find root families
        HashSet<string> childIds = new HashSet<string>();
        foreach (var family in entityStorageSignals.onGetFamilies().Values)
        {
            foreach (var child in family.Children)
            {
                childIds.Add(child);
            }
        }
        Vector3 startPosition = Vector3.zero;

        // Place root families
        foreach (var family in entityStorageSignals.onGetFamilies().Values)
        {

            if (!childIds.Contains(family.Id)) // Root family
            {
                startPosition += Vector3.right * FamilyHorizontalSpacing * 2; // Shift root families to the right
                PlaceFamily(family, startPosition, entityStorageSignals.onGetIndividuals(), entityStorageSignals.onGetFamilies());
            }
        }

        //OrderFamilies();
        SetNodes();
        OrderFamilies();
        PlaceUnfamiliedIndividuals();
        DrawLines();
    }

    private void PlaceFamily(Family family, Vector3 position, Dictionary<string, Individual> individuals, Dictionary<string, Family> families)
    {
        GameObject newFamily = new GameObject();
        createdFamilies[family.Id] = newFamily;
        newFamily.transform.position = position;
        Vector3 familyPosition = position;
        if (family.Husband != "" && individuals.ContainsKey(family.Husband))
        {
            PlaceIndividual(family.Husband, familyPosition, individuals, newFamily);
        }
        if (family.Wife != "" && individuals.ContainsKey(family.Wife))
        {
            PlaceIndividual(family.Wife, familyPosition + Vector3.right * IndividualHorizontalSpacing, individuals, newFamily);
        }
    }

    private void PlaceIndividual(string individualId, Vector3 position, Dictionary<string, Individual> individuals, GameObject familyObject)
    {
        // Create individual node
        GameObject individualNode = Instantiate(individuals[individualId].Gender == "M"? ManPrefab: WomanPrefab, position, Quaternion.identity);
        individualNode.GetComponent<CharacterController>().SetData(individuals[individualId]);
        individualNode.name = individualId;
        individualNode.transform.parent = familyObject.transform;
        familyObject.name = individuals[individualId].FAMS;

        allIndividualGameObjects.Add(individualId, individualNode);
    }

    private void SetNodes()
    {
        foreach (var createdFamilyId in createdFamilies.Keys)
        {
            foreach (var individual in entityStorageSignals.onGetIndividuals())
            {
                if (individual.Value.Gender != "M")
                {
                    continue;
                }
                int nodeCount = 0;
                if (createdFamilyId == individual.Value.FAMS)
                {
                    Individual indi = individual.Value;
                    while (indi.FAMC != null)
                    {
                        ++nodeCount;

                        indi = entityStorageSignals.onGetIndividuals()[entityStorageSignals.onGetFamilies()[indi.FAMC].Husband];
                    }

                    nodesInteractionCounts[createdFamilyId] = nodeCount;
                    if (nodeCount > maxConnectionCount)
                    {
                        maxConnectionCount = nodeCount;
                    }
                }
            }
        }

        foreach (var i in nodesInteractionCounts)
        {
            //Debug.Log(i.Key + " " + i.Value);
        }
    }

    private void OrderFamilies()
    {
        //Hiyerarþi listesi oluþtur. altlardakilerin countlarý daha fazla iken üstlerdekilerin daha az olacak. Önce az olanlarý oluþturup kademe kademe alta inebilirsin.

        Dictionary<string,Family> families = entityStorageSignals.onGetFamilies();
        Dictionary<string,Individual> individuals = entityStorageSignals.onGetIndividuals();

        for (int i = 0; i < maxConnectionCount + 2; i++)
        {
            foreach (var familyInteractionCountDictionary in nodesInteractionCounts)
            {
                if (familyInteractionCountDictionary.Value == i)
                {
                    //ailenin pozisyonu, o ailenin husbýnýn babasýnýn ailesinin pozisyonuna eþitlenir.
                    string husbId = families[familyInteractionCountDictionary.Key].Husband;
                    string husbFamc = individuals[husbId].FAMC;

                    if (husbFamc == null)
                    {
                        continue;
                    }

                    Family husbFamcFamily = families[husbFamc];
                    int familyChildCount = families[husbFamcFamily.Id].ChildrenToShow.Count;
                    float startPoint = 0f;
                    int husbandFamilyChildIndex = families[husbFamcFamily.Id].ChildrenToShow.IndexOf(husbId);

                    if (familyChildCount > 1)
                    {
                        startPoint = ((familyChildCount / 2) - 0.5f) * -1;
                    }

                    createdFamilies[familyInteractionCountDictionary.Key].transform.position = createdFamilies[husbFamcFamily.Id].transform.position + new Vector3(startPoint + 1 * husbandFamilyChildIndex * FamilyHorizontalSpacing, 0, 10);
                }
            }
        }
    }

    private void PlaceUnfamiliedIndividuals()
    {
        Dictionary<string, Family> families = entityStorageSignals.onGetFamilies();
        Dictionary<string, Individual> individuals = entityStorageSignals.onGetIndividuals();

        foreach (var i in individuals)
        {
            if (i.Value.FAMS == null)
            {
                GameObject individualNode = Instantiate(individuals[i.Key].Gender == "M" ? ManPrefab : WomanPrefab, Vector3.zero, Quaternion.identity);
                individualNode.GetComponent<CharacterController>().SetData(individuals[i.Key]);
                individualNode.name = i.Key;

                string husbFamc = individuals[i.Key].FAMC;
                Family husbFamcFamily = families[husbFamc];
                int familyChildCount = families[husbFamcFamily.Id].ChildrenToShow.Count;
                float startPoint = 0f;
                int husbandFamilyChildIndex = families[husbFamcFamily.Id].ChildrenToShow.IndexOf(i.Key);

                if (familyChildCount > 1)
                {
                    startPoint = (((familyChildCount / 2) - 0.5f) * -1);
                }
                individualNode.transform.position = createdFamilies[husbFamcFamily.Id].transform.position + new Vector3(startPoint + husbandFamilyChildIndex * FamilyHorizontalSpacing, 0, 10);

                allIndividualGameObjects.Add(i.Key, individualNode);
                //individualNode.GetComponent<LineRenderer>().SetPosition(0, individualNode.transform.position);
                //individualNode.GetComponent<LineRenderer>().SetPosition(1, createdFamilies[husbFamcFamily.Id].transform.position);
            }
        }
    }

    private void DrawLines()
    {
        foreach (var i in allIndividualGameObjects)
        {
            if (entityStorageSignals.onGetIndividuals()[i.Key].FAMC != null)
            {
                string husbFamc = entityStorageSignals.onGetIndividuals()[i.Key].FAMC;

                i.Value.GetComponent<LineRenderer>().SetPosition(0, i.Value.transform.position);
                i.Value.GetComponent<LineRenderer>().SetPosition(1, createdFamilies[husbFamc].transform.position);
            }
            else
            {
                i.Value.GetComponent<LineRenderer>().positionCount = 0;
            }
        }
    }
}
