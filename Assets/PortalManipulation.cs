using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManipulation : MonoBehaviour
{
    public GameObject originPortal;
    public GameObject originPortal_origin;
    public GameObject targetPortal;
    public MyVRPortal myVrPortal;
    public GameObject leftHand;
    public GameObject rightHand;

    private float _startScaleDistance = 1;


    private bool _isActive = false;
    private bool _prevIsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initalize(GameObject originPortal, GameObject targetPortal, MyVRPortal myVrPortal, GameObject leftHand, GameObject rightHand)
    {
        this.originPortal = originPortal;
        this.originPortal_origin = originPortal.transform.Find("PortalCircle_origin").gameObject;
        this.targetPortal = targetPortal;
        this.myVrPortal = myVrPortal;
        this.leftHand = leftHand;
        this.rightHand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessActive();


        if (_isActive)
        {
            if (_prevIsActive != _isActive)
            {
                _startScaleDistance = GetDistance(leftHand.transform.position, rightHand.transform.position);

 

                Debug.Log("Start!");
            }
            ProcessScale();
            ProcessPosition();
            ProcessRotation();
        }
        
        _prevIsActive = _isActive;
    }

    private void ProcessActive()
    {
        if (leftHand == null || rightHand == null)
        {
            return;
        }

        if (!leftHand.GetComponent<MyRealHand>().IsGrabbing() || !rightHand.GetComponent<MyRealHand>().IsGrabbing())
        {
            _isActive = false;
            return;
        }

        if (!myVrPortal.CollisionWith(leftHand) || !myVrPortal.CollisionWith(rightHand))
        {
            return;
        }

        _isActive = true;
    }

    private void ProcessScale()
    {

        if (_isActive != _prevIsActive)
        {
            StartScale();
        }
        SetScale();
    }
    private void ProcessPosition()
    {
        SetPosition();
    }

    //private void SetLossyScale()
    //{
    //    Matrix4x4 camera_to_world = originPortal.transform.localToWorldMatrix;
    //    this._lossyScale = new Vector3(camera_to_world.lossyScale.x, camera_to_world.lossyScale.y, camera_to_world.lossyScale.z);
    //}

    private float GetDistance(Vector3 point1, Vector3 point2)
    {
        return Vector3.Distance(
                myVrPortal.ProjectToPoint(point1),
                myVrPortal.ProjectToPoint(point2)
            );
    }
    private Vector3 GetAveragePoint(Vector3 point1, Vector3 point2)
    {
        return Vector3.Lerp(point1, point2, 0.5f);
        //return Vector3.Lerp(myVrPortal.ProjectToPoint(point1), myVrPortal.ProjectToPoint(point2), 0.5f);
    }

    // Scale Methods
    private void StartScale()
    {
        this._startScaleDistance = GetDistance(leftHand.transform.position, rightHand.transform.position);
    }

    private void SetScale()
    {
        float currentDistance = GetDistance(leftHand.transform.position, rightHand.transform.position);
        float distanceRatio = currentDistance / _startScaleDistance;
        originPortal.transform.localScale = new Vector3(this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio);
    }

    // Position
    private void SetPosition()
    {
        originPortal.transform.position = GetAveragePoint(leftHand.transform.position, rightHand.transform.position);
    }
    //rotation

    private Vector3 YawPosition(Vector3 p)
    {
        return new Vector3(p.x, 0, p.z);
    }
    private void ProcessRotation()
    {
        Vector3 leftHandPosition = leftHand.transform.position;
        Vector3 rightHandPosition = rightHand.transform.position;

        Quaternion rotation = Quaternion.LookRotation(YawPosition(rightHandPosition) - YawPosition(leftHandPosition), Vector3.up);
        
        originPortal.transform.rotation = rotation;
        originPortal.transform.Rotate(Vector3.up, 90f);
        //originPortal.transform.Rotate(Vector3.forward, 90f);
    }
}
