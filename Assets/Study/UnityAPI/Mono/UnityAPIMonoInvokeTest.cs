using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPIMonoInvokeTest : MonoBehaviour
{
    private void Start()
    {
        
    }

    public void TestInvoke(UnityAPIMonoInvokeTestInput unityAPIMonoInvokeTestInput)
    {
        unityAPIMonoInvokeTestInput.Invoke("DebugLogSthing", 2);
    }

    public void TestInvoke()
    {
        Invoke("DebugLogSthing", 2);
    }

    void DebugLogSthing()
    {
        Debug.Log("执行了");
    }
}
