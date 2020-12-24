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
        /// 结论 GetCopyOf
        /// </summary>
        #region GetCopyOf

        [Test]
        public void TestGetCopyOf_CreatGameobjectThenAddComponent_True()
        {
            Camera.main.fieldOfView = 20;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Camera temp = cube.AddComponent<Camera>();
            temp.enabled = false;
            temp = temp.GetCopyOf(Camera.main);
            // temp.CopyFrom(Camera.main);
            Assert.AreEqual(20, temp.fieldOfView);
            temp.fieldOfView = 30;
            Camera.main.fieldOfView = 50;
            Camera.main.CopyFrom(temp);
            Assert.AreEqual(30, Camera.main.fieldOfView);
        }

        [Test]
        public void TestGetCopyOf_NewComponent_False()
        {
            Camera.main.fieldOfView = 20;
            Camera temp = new Camera();
            temp.CopyFrom(Camera.main);
            Assert.AreEqual(20, temp.fieldOfView);
            temp.fieldOfView = 30;
            Camera.main.fieldOfView = 50;
            Camera.main.CopyFrom(temp);
            Assert.AreEqual(30, Camera.main.fieldOfView);
        }

        #endregion

    }
}
