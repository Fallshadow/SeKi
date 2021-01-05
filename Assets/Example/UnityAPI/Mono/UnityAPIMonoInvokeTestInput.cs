using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPIMonoInvokeTestInput : MonoBehaviour
{
    public UnityAPIMonoInvokeTest invokeTest;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            invokeTest.TestInvoke();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            invokeTest.TestInvoke(this);
        }
    }
}
