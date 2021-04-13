using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnityEngineVector3Distance : MonoBehaviour
{
    public GameObject cube0;
    public GameObject cube1;
    
    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(cube0.transform.position, cube1.transform.position));
    }
}
