using UnityEngine;
using UnityEngine.UI;

public class UIGameGetPoints : MonoBehaviour
{
    private Text            myText = null;

    private HouseCheck[]    myHouseCheck = null;

    private void Start()
    {
        myText = GetComponent<Text>();
        myHouseCheck = FindObjectsOfType<HouseCheck>();
    }

    void Update()
    {
        int score = 0;
        for(int i = 0; i < myHouseCheck.Length; i++)
        {
            score += myHouseCheck[i].GetTotalPoints();
        }
        myText.text = score.ToString();
    }
}
