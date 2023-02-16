using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WIMManager : MonoBehaviour
{
   public GameObject worldObj;
   public GameObject experimentObj;

   private GameObject worldObj_instance;
   private GameObject experimentObj_instance;

   private static WIMManager instance;

   private void Awake(){
       instance = this;
   }

   private void Start(){
       if(worldObj == null){

       }else{
           worldObj_instance = Instantiate(worldObj, new Vector3(1,1,1), transform.rotation);
           worldObj_instance.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
       }

       if(experimentObj == null){

       }else{
           experimentObj_instance = Instantiate(worldObj, new Vector3(1,1,1), transform.rotation);
           experimentObj_instance.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
       }
   }

    private bool IsNewObjectThere(){
        if(experimentObj.transform.childCount != experimentObj_instance.transform.childCount){
            return true;
        }

        return false;
    }

   private void Update(){
    //    if(IsNewObjectThere()){
           
    //    }
   }

}
