using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNewTrial : MonoBehaviour
{
    public GameObject dockingTask;
    public GameObject selectionTask;

    private void startNewTrial()
    {
        if (Experiment_Setting.instance.task_type == Task_Type.DOCKING_TASK)
        {
            dockingTask.SetActive(true);
        }
        else if (Experiment_Setting.instance.task_type == Task_Type.SELECTION_TASK)
        {
            selectionTask.SetActive(true);
        }
        else
        {
            // do nothing
        }
    }

    public void TriggerStartNewTrial(){
        startNewTrial();
        gameObject.SetActive(false);
    }

}
