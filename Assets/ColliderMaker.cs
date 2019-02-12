using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMaker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in this.GetComponentInChildren<Transform>()) {
            MeshCollider collider = child.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = child.gameObject.GetComponent<MeshFilter>().mesh;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
