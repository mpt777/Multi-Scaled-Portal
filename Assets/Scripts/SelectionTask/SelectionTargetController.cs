using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTargetController : MonoBehaviour
{
    public Material mr_target_normal;
    public Material mr_target_active;
    public Material mr_target_is_contacting;
    
    [SerializeField]
    private bool isTarget = false;
    private bool isClicked = false;

    public void initTarget()
    {
        // Debug.Log("initTarget Called");
        isTarget = false;
        isClicked = false;
        transform.GetComponent<Renderer>().material = mr_target_normal;
    }

    public void setIsTarget(bool val)
    {
        isTarget = val;

        if(val){
            transform.GetComponent<Renderer>().material = mr_target_active;
        }else{
            transform.GetComponent<Renderer>().material = mr_target_normal;
        }
    }

    public bool getIsTarget()
    {
        return isTarget;
    }

    public void setIsClicked(bool val)
    {
        isClicked = val;
    }

    public bool getIsClicked()
    {
        return isClicked;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        initTarget();
    }

    public void TurnTargetIsContacting(){
        transform.GetComponent<Renderer>().material = mr_target_is_contacting;
    }

    public void TurnTargetIsNotContacting(){
        if(isTarget){
            transform.GetComponent<Renderer>().material = mr_target_active;
        }else{
            transform.GetComponent<Renderer>().material = mr_target_normal;
        }
    }
}
