using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron_test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
    }

    void OnTriggerEnter(Collision other) {
    }   

    void Start(){
     Mesh mesh = GetComponent<MeshFilter>().mesh;
     Vector3[] vertices = mesh.vertices;
     for (var i = 0; i < vertices.Length; i++)
     {
         //Debug.Log(vertices[i]);
         var direction = transform.TransformPoint(vertices[i]);
         //Debug.Log(direction);
     }
    }
}
