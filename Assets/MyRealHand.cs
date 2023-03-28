using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRealHand : MonoBehaviour
{

    public GameObject myPortalHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMyPortalHand(GameObject? obj)
    {
        myPortalHand = obj;
    }
}
