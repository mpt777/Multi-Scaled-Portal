using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManipulation : MonoBehaviour
{
    public GameObject originPortal;
    public GameObject targetPortal;
    public MyVRPortal myVrPortal;
    public GameObject leftHand;
    public GameObject rightHand;

    private float _startScaleDistance = 1;
    private Vector3 _lossyScale = new Vector3(1, 1, 1);

    private bool _isActive = false;
    private bool _prevIsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initalize(GameObject originPortal, GameObject targetPortal, MyVRPortal myVrPortal, GameObject leftHand, GameObject rightHand)
    {
        this.originPortal = originPortal;
        this.targetPortal = targetPortal;
        this.myVrPortal = myVrPortal;
        this.leftHand = leftHand;
        this.rightHand = rightHand;
    }

    // Update is called once per frame
    void Update()
    {
        if (leftHand == null || rightHand == null)
        {
            return;
        }
        ProcessActive();
        if (_isActive)
        {
            ProcessGrabbing();
        }
        
        _prevIsActive = _isActive;
    }

    private void ProcessActive()
    {

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

    private void ProcessGrabbing()
    {

        if (_isActive != _prevIsActive)
        {
            StartScale();
            Debug.Log("Start Scale");
        }
        ProcessScale();
    }

    private void SetLossyScale()
    {
        Matrix4x4 camera_to_world = originPortal.transform.localToWorldMatrix;
        this._lossyScale = new Vector3(camera_to_world.lossyScale.x, camera_to_world.lossyScale.y, camera_to_world.lossyScale.z);
    }
    private float GetDistance2(Vector3 point1, Vector3 point2)
    {

        return Vector3.Distance(
                myVrPortal.ProjectToPoint(point1),
                myVrPortal.ProjectToPoint(point2)
            );
    }
    private void StartScale()
    {
        this._startScaleDistance = GetDistance2(leftHand.transform.position, rightHand.transform.position);
    }

    private void ProcessScale()
    {
        float currentDistance = GetDistance2(leftHand.transform.position, rightHand.transform.position);
        float distanceRatio = currentDistance / _startScaleDistance;
        originPortal.transform.localScale = new Vector3(this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio);
        //targetPortal.transform.localScale = new Vector3(this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio, this._startScaleDistance * distanceRatio);
    }
}
