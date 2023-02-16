using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierLine : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform point0, point1, point_mid;
    
    private int numPoints = 50;
    private Vector3[] positions = new Vector3[50];



    private Vector3 CalculateLinearBezierPoint(float t, Vector3 p0, Vector3 p1){
        return p0 + t * (p1 - p0);
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p_mid){
        return (1-t)*((1-t)*p0 + t*p_mid) + t*((1-t)*p_mid + t*p1);
    }

    // Start is called before the first frame update
    void Start()
    {
        // lineRenderer.SetVertexCount(numPoints);
        lineRenderer.positionCount = numPoints;
        DrawQuadraticCurve();
    }

    private void DrawLinearCurve(){
        for(int i=1; i < numPoints+1; i++){
            float t = i / (float) numPoints;
            // Vector3 point = CalculateLinearBezierPoint(t, )
            positions[i-1] = CalculateLinearBezierPoint(t, point0.position, point1.position );
        }

        lineRenderer.SetPositions(positions);
    }

    private void DrawQuadraticCurve(){
        for(int i=1; i < numPoints+1; i++){
            float t = i / (float) numPoints;
            // Vector3 point = CalculateLinearBezierPoint(t, )
            positions[i-1] = CalculateQuadraticBezierPoint(t, point0.position, point1.position, point_mid.position);
        }

        lineRenderer.SetPositions(positions);
    }

    // Update is called once per frame
    void Update()
    {
        DrawQuadraticCurve();
    }
}
