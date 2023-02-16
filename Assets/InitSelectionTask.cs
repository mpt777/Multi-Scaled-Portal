using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSelectionTask : MonoBehaviour
{
    public GameObject centerObj;

    public float fitts_law_radius;
    public float target_obj_size;
    public GameObject[] fitts_law_objects;
    private int fitts_law_start_idx = 0;
    private int[] fitts_law_order = new int[] {8,0,9,1, 10, 2,11,3,12,4,13,5,14,6,15,7,16};
    // Start is called before the first frame update

    public int[] GetSelectionOrder(){
        return fitts_law_order;
    }

    public GameObject[] GetFittsLawObjects(){
        return fitts_law_objects;
    }

    private void OnEnable()
    {
        Vector3 centerPos = centerObj.transform.position;
        int numOfTarget = fitts_law_objects.Length;

        int half_numOfTarget = numOfTarget / 2;
        int num_front_obj =0;
        int num_back_obj = 0;

        for (int i=0; i< numOfTarget; i++)
        {
            float curTheta = - i * 2.0f * Mathf.PI / numOfTarget + Mathf.PI/2;
            
            // float offset = Random.Range(-0.1f, 0.1f);
            float offset = 1.0f;

            if((float) i < numOfTarget / 2){
                offset = -1.0f;
            }else{
                offset = 1.0f;
            }


            // change the Random range 
            float posX = centerPos.x + offset * Experiment_Setting.instance.reach_offset_for_selectionTask / 2;
            //  float posX = centerPos.x + offset * 0.9f / 2;
            float posY = centerPos.y + fitts_law_radius * Mathf.Sin(curTheta);
            float posZ = centerPos.z + fitts_law_radius * Mathf.Cos(curTheta);
            Vector3 targetPos = new Vector3(posX, posY, posZ);

            // Instantiate
            GameObject target = fitts_law_objects[i];
            target.GetComponent<Renderer>().material = target.GetComponent<SelectionTargetController>().mr_target_normal;
           
            target.transform.position = targetPos;

            if(target_obj_size == 0){
                target_obj_size = 0.05f;
            }
            
            target.transform.localScale = new Vector3(target_obj_size, target_obj_size, target_obj_size);
        }
    }
}
