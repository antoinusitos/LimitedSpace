using UnityEngine;
using UnityEngine.UI;

public class SecondaryObjectiveManager : MonoBehaviour
{
    private SecondaryObjective[] mySecondaryObjectives = null;

    [SerializeField]
    private GameObject              myUIToSpawn = null;
    private GameObject[]            myUISpawned = null;

    [SerializeField]
    private Transform               myPanel = null;

    private void Start()
    {
        mySecondaryObjectives = FindObjectsOfType<SecondaryObjective>();
        myUISpawned = new GameObject[mySecondaryObjectives.Length];
        
        for(int i = 0; i < mySecondaryObjectives.Length; i++)
        {
            myUISpawned[i] = Instantiate(myUIToSpawn, myPanel);
            myUISpawned[i].GetComponentInChildren<Text>().text = mySecondaryObjectives[i].GetMyText();
        }

    }

    private void Update()
    {
        for(int i = 0; i < mySecondaryObjectives.Length; i++)
        {
            if(mySecondaryObjectives[i].GetMyStateChanged())
            {
                myUISpawned[i].GetComponentInChildren<Toggle>().isOn = mySecondaryObjectives[i].GetObjectiveComplete();
            }
        }
    }
}
