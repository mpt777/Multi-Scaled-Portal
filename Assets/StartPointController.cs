/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartPointController : MonoBehaviour
{
    
    // selection task setting
    public Material mr_target_before;
    public Material mr_target_after;
    public GameObject targets;

    // docking task setting



    // define 3 public variables - we can then assign their color values in the inspector.
    public Color red;
    public Color amber;
    public Color green;

    // reference to the material we want to change the color of.
    Material material;

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {
        // get the material that is used to render this object (via the MeshRenderer component)
        material = GetComponent<MeshRenderer>().material;
    }

    IEnumerator SelectionTaskCountdown()
    {
        yield return new WaitForSeconds(0.5f);
        // change one of targets colot from MR_TargetBefore tp MR_TargetAfter
        int totalNumOfTargets = targets.GetComponent<InitSelectionTask>().numOfTarget;
        int rand = (int)Mathf.Floor(Random.Range(0, totalNumOfTargets - 0.0001f));

        GameObject targetObj = targets.transform.GetChild(rand).gameObject;
        targetObj.GetComponent<MeshRenderer>().material = mr_target_after;
        targetObj.GetComponent<SelectionTargetController>().setIsTarget(true);

        session.BeginNextTrial(); // <-- new
    }

    /// OnTriggerEnter is called when the Collider 'other' enters the trigger.
    void OnTriggerEnter(Collider other)
    {
        // Todo: change the color when VR input trigger on
        

        // if current experiment is selection,
        if (!session.InTrial) {
            material.color = amber;
            StartCoroutine(SelectionTaskCountdown());
         }

    }

    /// OnTriggerExit is called when the Collider 'other' has stopped touching the trigger.
    void OnTriggerExit(Collider other)
    {
        StopAllCoroutines();

        // Todo: not necessary
        material.color = red;
    }
}*/