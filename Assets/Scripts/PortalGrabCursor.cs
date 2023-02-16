using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGrabCursor : MonoBehaviour
{
  public bool isGrabbed;

  public void SetIsGrabbed(bool val){
      isGrabbed = val;
  }

  public bool GetIsGrabbed(){
      return isGrabbed;
  }

  public void ActivateCursor(){
      transform.GetComponent<Renderer>().enabled = true;
  }

  public void DeactivateCursor(){
      transform.GetComponent<Renderer>().enabled = false;
  }
}
