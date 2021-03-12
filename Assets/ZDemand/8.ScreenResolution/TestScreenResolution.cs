using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TestScreenResolution : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // 只在构建中生效哦
            Screen.SetResolution(19200, 10800, false);
        }
    }
}