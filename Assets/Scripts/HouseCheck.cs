using System.Collections.Generic;
using UnityEngine;

public class HouseCheck : MonoBehaviour
{
    private List<Takeable>  myTakeableInside = new List<Takeable>();

    private int             myTotalPoints = 0;

    private void OnTriggerEnter(Collider other)
    {
        Takeable takeable = other.GetComponentInParent<Takeable>();
        if(takeable != null)
        {
            if (!myTakeableInside.Contains(takeable))
            {
                myTakeableInside.Add(takeable);
                myTotalPoints += takeable.GetPoints();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Takeable takeable = other.GetComponentInParent<Takeable>();
        if (takeable != null)
        {
            if(myTakeableInside.Contains(takeable))
            {
                myTotalPoints -= takeable.GetPoints();
                myTakeableInside.Remove(takeable);
            }
        }
    }

    public int GetTotalPoints()
    {
        return myTotalPoints;
    }
}
