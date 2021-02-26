using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPITestTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Time.time游戏开始的总时间:{Time.time}");
        Debug.Log($"Time.frameCount游戏开始的总帧数:{Time.frameCount}");
        Debug.Log($"Time.smoothDeltaTime经过平滑处理的:{Time.smoothDeltaTime}");
    }
}
