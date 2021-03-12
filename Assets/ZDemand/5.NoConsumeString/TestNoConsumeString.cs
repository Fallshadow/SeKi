using System;
using System.Text;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TestNoConsumeString : MonoBehaviour
    {
        public string TestStr = "";

        public byte[] bytes = new Byte[8];

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                TestStr = "我去";
            }

            if (Input.GetKey(KeyCode.B))
            {
                string newString = "我去";
                newString = "哈哈";
            }

            if (Input.GetKey(KeyCode.A))
            {
                TestStr = Encoding.UTF8.GetString(bytes);
            }
        }
    }
}