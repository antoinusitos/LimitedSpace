using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField]
    private float       myTimeOfGame = 60;

    private float       myCurrentTime = 0;

    [SerializeField]
    private Text        myTextTime = null;

    [SerializeField]
    private Text        myTextFinish = null;

    [SerializeField]
    private HouseCheck  myHouseCheck = null;

    private void Start()
    {
        myCurrentTime = myTimeOfGame;
    }

    // Update is called once per frame
    void Update()
    {
        myCurrentTime -= Time.deltaTime;

        if(myCurrentTime <= 0)
        {
            myCurrentTime = 0;
            myTextFinish.gameObject.SetActive(true);
            myTextFinish.text = "TOTAL SCORE " + myHouseCheck.GetTotalPoints().ToString();
        }

        myTextTime.text = myCurrentTime.ToString();
    }
}
