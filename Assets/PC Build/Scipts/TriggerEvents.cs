using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvents : MonoBehaviour
{
    public bool forSpecificObject = false;
    public Collider targetCollider;
    //public EggSaltScript eggSaltScript;  // Reference to the EggSaltScript

    public UnityEvent triggerEnter;
    public UnityEvent triggerStay;
    public UnityEvent triggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (forSpecificObject)
        {
            if (other == targetCollider)
            {
                triggerEnter.Invoke();
                //eggSaltScript.IncreaseDensity(); // Call IncreaseDensity when the specific collider triggers
            }
        }
        else
        {
            triggerEnter.Invoke();
            //eggSaltScript.IncreaseDensity(); // Call IncreaseDensity for any collider that triggers
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (forSpecificObject)
        {
            if (other == targetCollider)
            {
                triggerStay.Invoke();
            }
        }
        else
        {
            triggerStay.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (forSpecificObject)
        {
            if (other == targetCollider)
            {
                triggerExit.Invoke();
            }
        }
        else
        {
            triggerExit.Invoke();
        }
    }
}
