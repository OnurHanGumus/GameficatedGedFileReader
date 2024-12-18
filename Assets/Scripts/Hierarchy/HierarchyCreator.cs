using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HierarchyCreator : MonoBehaviour
{
    public GameObject WomanPrefab; // Prefab for individual nodes
    public GameObject ManPrefab; // Prefab for individual nodes
    public GameObject FamilyPrefab; // Prefab for individual nodes

    public Material LineMaterial; // Material for LineRenderer

    public float FamilyHorizontalSpacing = 2f; // Spacing between nodes
    public float IndividualHorizontalSpacing = 1f; // Spacing between nodes
    public int ParentNodeSpacing = 20;
    [SerializeField] private EntityStorageSignals entityStorageSignals;
    private Dictionary<string, GameObject> createdFamilies = new Dictionary<string, GameObject>();
    private Dictionary<string, int> nodesInteractionCounts = new Dictionary<string, int>();
    private Dictionary<string, GameObject> allIndividualGameObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> otherIndividualGameObjects = new Dictionary<string, GameObject>();
    private int maxConnectionCount = 0;
    Dictionary<string, Family> families;
    Dictionary<string, Individual> individuals;

    public void Start()
    {
        Init();
        Invoke("CreateHierarchy", 1f);
    }

    private void Init()
    {
        families = entityStorageSignals.onGetFamilies();
        individuals = entityStorageSignals.onGetIndividuals();
    }

    public void CreateHierarchy()
    {
        CreateFamiliesAndIndividuals();
        SetFamiliesInteractionCounts();
        OrderFamilies();
        PlaceUnfamiliedIndividuals();
        SetDivorcedFamilyNodes();

        ArrangeParentNodes();
        DrawLines(allIndividualGameObjects);
        DrawLines(otherIndividualGameObjects);
    }

    private void CreateFamiliesAndIndividuals()
    {
        foreach (var family in families.Values)
        {
            PlaceFamily(family, Vector3.zero, individuals, families);
        }
    }

    private void PlaceFamily(Family family, Vector3 position, Dictionary<string, Individual> individuals, Dictionary<string, Family> families)
    {
        GameObject newFamily = new GameObject();
        createdFamilies[family.Id] = newFamily;
        newFamily.transform.position = position;
        newFamily.name = family.Id;

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
        GameObject preferedPrefab = null;
        if (individuals[individualId].Gender == "M")
        {
            preferedPrefab = ManPrefab;
        }
        else if (individuals[individualId].Gender == "F")
        {
            preferedPrefab = WomanPrefab;

        }
        else
        {
            preferedPrefab = FamilyPrefab;
        }

        GameObject individualNode = Instantiate(preferedPrefab, position, Quaternion.identity);

        individualNode.GetComponent<CharacterController>().SetData(individuals[individualId]);
        individualNode.name = individualId;
        individualNode.transform.parent = familyObject.transform;

        if (allIndividualGameObjects.ContainsKey(individualId))
        {
            if (!otherIndividualGameObjects.ContainsKey(individualId))
            {
                otherIndividualGameObjects.Add(individualId, individualNode);
            }
            return;
        }
        allIndividualGameObjects.Add(individualId, individualNode);
    }

    private void SetFamiliesInteractionCounts()
    {
        foreach (var createdFamilyId in createdFamilies.Keys)
        {
            foreach (var individual in individuals)
            {
                if (individual.Value.Gender == "F")
                {
                    continue;
                }
                int nodeCount = 0;
                if (createdFamilyId == individual.Value.FAMS)
                {
                    CalculateFamilyInteractionCount(nodeCount, individual.Value, createdFamilyId);
                }
            }
        }
    }

    private void CalculateFamilyInteractionCount(int nodeCount, Individual indi, string familyId)
    {
        while (indi.FAMC != null)
        {
            ++nodeCount;
            if (!individuals.ContainsKey(families[indi.FAMC].Husband))
            {
                break;
            }
            indi = individuals[families[indi.FAMC].Husband];
        }
        nodesInteractionCounts[familyId] = nodeCount;

        if (nodeCount > maxConnectionCount)
        {
            maxConnectionCount = nodeCount;
        }
    }

    private void SetDivorcedFamilyNodes()
    {
        foreach (var family in entityStorageSignals.onGetFamilies())
        {
            if (!nodesInteractionCounts.ContainsKey(family.Key))
            {
                CalculateFamilyInteractionCount(0, individuals[families[family.Key].Husband], family.Key);
            }
        }
    }

    private void OrderFamilies()
    {
        //Hiyerarþi listesi oluþtur. altlardakilerin countlarý daha fazla iken üstlerdekilerin daha az olacak. Önce az olanlarý oluþturup kademe kademe alta inebilirsin.

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

                    if (familyChildCount > 0)
                    {
                        startPoint = PositionCalculator(familyChildCount);
                    }

                    createdFamilies[familyInteractionCountDictionary.Key].transform.position = createdFamilies[husbFamcFamily.Id].transform.position + new Vector3(startPoint + husbandFamilyChildIndex * FamilyHorizontalSpacing, 0, 10);
                    createdFamilies[familyInteractionCountDictionary.Key].transform.parent = createdFamilies[husbFamcFamily.Id].transform;
                }
            }
        }
    }

    private void PlaceUnfamiliedIndividuals()
    {
        foreach (var i in individuals)
        {
            if (i.Value.FAMS == null)
            {
                GameObject individualNode = Instantiate(individuals[i.Key].Gender == "M" ? ManPrefab : WomanPrefab, Vector3.zero, Quaternion.identity);
                individualNode.GetComponent<CharacterController>().SetData(individuals[i.Key]);
                individualNode.name = i.Key;

                string husbFamc = individuals[i.Key].FAMC;
                if (individuals[i.Key].FAMC =="")
                {
                    continue;
                }
                Family husbFamcFamily = families[husbFamc];
                int familyChildCount = families[husbFamcFamily.Id].ChildrenToShow.Count;
                float startPoint = 0f;
                int husbandFamilyChildIndex = families[husbFamcFamily.Id].ChildrenToShow.IndexOf(i.Key);

                if (familyChildCount > 0)
                {
                    startPoint = PositionCalculator(familyChildCount);
                }
                individualNode.transform.position = createdFamilies[husbFamcFamily.Id].transform.position + new Vector3(startPoint + husbandFamilyChildIndex * FamilyHorizontalSpacing, 0, 10);
                if (allIndividualGameObjects.ContainsKey(i.Key))
                {
                    continue;
                }
                allIndividualGameObjects.Add(i.Key, individualNode);
                individualNode.transform.parent = createdFamilies[husbFamcFamily.Id].transform;
            }
        }
    }

    private float PositionCalculator(int familyChildCount)
    {

        return (((familyChildCount / 2) - 0.5f) * -1) * FamilyHorizontalSpacing;
    }

    private void DrawLines(Dictionary<string,GameObject> individualDictionary)
    {
        foreach (var i in individualDictionary)
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

    private void ArrangeParentNodes()
    {
        int offset = 0;
        foreach (var i in nodesInteractionCounts)
        {
            if (i.Value == 0)
            {
                Vector3 pos = createdFamilies[i.Key].transform.position;
                offset += ParentNodeSpacing;
                createdFamilies[i.Key].transform.position = new Vector3(offset, pos.y, pos.z);
            }
        }
    }
}
