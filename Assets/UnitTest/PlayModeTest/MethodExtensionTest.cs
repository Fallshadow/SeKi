using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MethodExtensionTest
    {
        /// <summary>
        /// 结论 GetCopyOf和copyfrom都是好样的
        /// </summary>
        #region GetCopyOf

        [Test]
        public void TestGetCopyOf_CreatGameobjectThenAddComponent_True()
        {
            // 代码API创建Cube
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Camera temp1 = cube1.AddComponent<Camera>();
            Camera temp2 = cube2.AddComponent<Camera>();
            temp1.fieldOfView = 30;
            temp2.fieldOfView = 20;
            temp1.GetCopyOf(temp2);
            Assert.AreEqual(20, temp1.fieldOfView);
        }
        
        [Test]
        public void TestCopyFrom_CreatGameobjectThenAddComponent_True()
        {
            // 代码API创建Cube
            GameObject cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Camera temp1 = cube1.AddComponent<Camera>();
            Camera temp2 = cube2.AddComponent<Camera>();
            temp1.fieldOfView = 30;
            temp2.fieldOfView = 20;
            temp1.CopyFrom(temp2);
            Assert.AreEqual(20, temp1.fieldOfView);
        }

        #endregion

    }
}
