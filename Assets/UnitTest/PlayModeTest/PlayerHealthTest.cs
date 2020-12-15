using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerHealthTest
    {
        [Test]
        public void TestHealth()
        {
            PlayerHealth playerHealth = new PlayerHealth();
            playerHealth.Health = 100;
            playerHealth.Damage(20);
            Assert.AreEqual(80, playerHealth.Health);
            playerHealth.Damage(-15);
            Assert.AreEqual(95, playerHealth.Health);
        }

        [Test]
        public void TestHealthWrong()
        {
            PlayerHealth playerHealth = new PlayerHealth();
            playerHealth.Health = 100;
            playerHealth.DamageWrong(20);
            Assert.AreEqual(80, playerHealth.Health);
            playerHealth.DamageWrong(-15);
            Assert.AreEqual(95, playerHealth.Health);
        }

        [Test]
        public void TestHealthNoException()
        {
            PlayerHealth playerHealth = new PlayerHealth();
            playerHealth.Health = 100;
            playerHealth.DamageNoException(20);
            Assert.AreEqual(80, playerHealth.Health);
            playerHealth.DamageNoException(-15);
            Assert.AreEqual(95, playerHealth.Health);
        }
    }
}
