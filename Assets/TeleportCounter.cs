using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCounter : MonoBehaviour
{
    public static TeleportCounter instance;
    int numTeleport;


    private void Awake(){
        instance = this;
    }

    public void ResetNumTeleport(){
        numTeleport = 0;
    }

    public void IncrementNumTeleport(){
        numTeleport++;
    }

    public int GetNumTeleport(){
        return numTeleport;
    }
}
