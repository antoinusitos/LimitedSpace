using UnityEngine;
using UnityEngine.UI;

public class UIGameGetPoints : MonoBehaviour
{
    private Text        myText = null;

    [SerializeField]
    private HouseCheck  myHouseCheck = null;

    private void Start()
    {
        myText = GetComponent<Text>();
    }

    void Update()
    {
        myText.text = myHouseCheck.GetTotalPoints().ToString();
    }
}
