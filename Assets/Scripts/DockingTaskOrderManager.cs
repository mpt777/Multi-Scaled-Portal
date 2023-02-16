using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockingTaskOrderManager : MonoBehaviour
{
    public Selection_Technique[] techniques;
    public Target_Distance[] session1_blocks;
    public Target_Distance[] session2_blocks;
    // public Target_Distance[] session3_blocks;


    public bool DoSeeThroughDockingTask;
    public static DockingTaskOrderManager instance;

    private void Awake(){
        instance = this;
    }
}
