using UnityEngine;

public class SecondaryObjective : MonoBehaviour
{
    [SerializeField]
    protected string      myText = "";

    protected bool        myObjectiveComplete = false;

    protected bool        myStateChanged = false;

    

    public bool GetObjectiveComplete()
    {
        return myObjectiveComplete;
    }

    public string GetMyText()
    {
        return myText;
    }

    public bool GetMyStateChanged()
    {
        bool state = myStateChanged;
        myStateChanged = false;
        return state;
    }
}
