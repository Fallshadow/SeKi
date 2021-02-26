using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPITestSample : MonoBehaviour
{
    void Update()
    {
        TestSample();
    }

    // 相当于将这中间的代码深度分析，用来测试性能
    public void TestSample()
    {
        UnityEngine.Profiling.Profiler.BeginSample("[Test]DebugLogEmptySample");
        ASeKi.debug.PrintSystem.Log("It's empty");
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
