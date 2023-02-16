using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DOCKING_OBJ_TYPE
{
    TARGET_OBJ, CONTROL_OBJ
}

public class DockingTaskObject : MonoBehaviour
{
    public DOCKING_OBJ_TYPE targetObj_type;
    
    public GameObject vertexA;
    public GameObject vertexB;
    public GameObject vertexC;
    public GameObject vertexD;

    private bool isSatisfyOffset = false;
    private bool isClicked = false;

    public Vector3[] GetVertices(){
        Vector3[] uniqueVertices = new Vector3[4];
        int curIndex = 0;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (var i = 0; i < vertices.Length; i++)
        {
            var direction = transform.TransformPoint(vertices[i]);

            bool isExist = false;

            for(int j=0; j<uniqueVertices.Length; j++){
                if(direction == uniqueVertices[j]){
                    isExist = true;
                    break;
                }
            }

            if(!isExist){
                uniqueVertices[curIndex] = direction;
                curIndex++;
            }
        }

        return uniqueVertices;
    }

    // Start is called before the first frame update
    private void onEnable()
    {
        init();
    }

    public void InitVertices(){
        Vector3[] vertices = GetVertices();
        vertexA.transform.position = vertices[0];
        vertexB.transform.position = vertices[1];
        vertexC.transform.position = vertices[2];
        vertexD.transform.position = vertices[3];
    }

    public void init()
    {
        isClicked = false;
        isSatisfyOffset = false;
    }

    public bool getIsTarget()
    {
        if (DOCKING_OBJ_TYPE.TARGET_OBJ == targetObj_type) return true;

        return false;
    }

    
    public bool getIsSatisfyOffset()
    {
        return isSatisfyOffset;
    }

    public void setIsSatisfyOffset(bool val)
    {
        isSatisfyOffset = val;
    }

    public bool getIsClicked()
    {
        return isClicked;
    }

    public void setIsClicked(bool val)
    {
        isClicked = val;
    }

    public Vector3 getCenterCoordinate(){
        return transform.position;
    }
}
