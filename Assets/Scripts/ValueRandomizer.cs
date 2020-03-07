using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueRandomizer : MonoBehaviour
{
    [SerializeField]
    private Takeable takeableComponent;

    [SerializeField]
    private bool changeMass;

    [SerializeField]
    private float minMassValue;

    [SerializeField]
    private float maxMassValue;

    [SerializeField]
    private bool changePoints;

    [SerializeField]
    private int minPointsValue;

    [SerializeField]
    private int maxPointsValue;

    private void Start()
    {
        if(takeableComponent != null)
        {
            if (changeMass)
            {
                takeableComponent.SetMass(Random.Range(minMassValue, maxMassValue));
            }

            if (changePoints)
            {
                takeableComponent.SetPoints(Random.Range(minPointsValue, maxPointsValue + 1));
            }
        }
        
    }
}
