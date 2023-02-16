using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class portalCameraManager : MonoBehaviour
{
    
    /*
    public Transform playerCamera;
    public Transform portal;
    public Transform otherPortal;

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = playerCamera.position - otherPortal.position;
        transform.position = portal.position + playerOffsetFromPortal;

        float angularDifferenceBetweenPortalRotations = Quaternion.Angle(portal.rotation, otherPortal.rotation);

        Quaternion portalRotationalDifference = Quaternion.AngleAxis(angularDifferenceBetweenPortalRotations, Vector3.up);
        Vector3 newCameraDirection = portalRotationalDifference * playerCamera.right *(-1) ;
        transform.rotation = Quaternion.LookRotation(newCameraDirection, Vector3.up);
    }
    */

    

    public Camera mainVRCamera;
    public GameObject otherSideCenterPivot;
    public GameObject thisSideCenterPivot;

    private void Start()
    {
        transform.GetComponent<Camera>().fieldOfView = mainVRCamera.fieldOfView;
    }

    Vector3 getRelativePosition()
    {
        Vector3 cameraPosition = mainVRCamera.transform.position;
        Vector3 otherCenterPivotPosition = otherSideCenterPivot.transform.position;

        return new Vector3(-cameraPosition.z + otherCenterPivotPosition.z,
            -cameraPosition.y + otherCenterPivotPosition.y,
            +cameraPosition.x - otherCenterPivotPosition.x);
    }

    Vector3 getMainCameraRotation()
    {
        return mainVRCamera.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        transform.localPosition = -getRelativePosition();
        transform.eulerAngles = getMainCameraRotation();
        
         this.GetComponent<Camera>().nearClipPlane = Vector3.Distance(mainVRCamera.transform.position, otherSideCenterPivot.transform.position);
         Debug.Log(Vector3.Distance(mainVRCamera.transform.position, otherSideCenterPivot.transform.position));
         */
        //Debug.Log("original" + Vector3.Distance(mainVRCamera.transform.position, otherSideCenterPivot.transform.position));
        //Debug.Log("teleport" + Vector3.Distance(transform.position, thisSideCenterPivot.transform.position));
    }

}
