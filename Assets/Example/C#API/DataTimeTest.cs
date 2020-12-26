using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTimeTest : MonoBehaviour
{
    void Update()
    {
        // Debug.Log($"{System.DateTime.Now.Hour.ToString()}:{System.DateTime.Now.Minute.ToString()}");
        // Debug.Log($"{System.DateTime.Now.ToString("HH:mm")}");
        Debug.Log($"{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff")}");
    }
}
