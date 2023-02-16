using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCenterController : MonoBehaviour
{
    private bool isWaiting = false;
    public Material mr_center_waiting;
    public Material mr_center_notwaiting;
    public float targetWaitTime = 3.0f;
    public float currentWaitTime = 0.0f;

    public void setIsWaiting(bool val)
    {
        isWaiting = val;
        if(val == true)
        {
            gameObject.GetComponent<Renderer>().material = mr_center_waiting;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material = mr_center_notwaiting;
        }
    }

    public bool getIsWaiting()
    {
        return isWaiting;
    }
   
}
