using Codigo.Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.TiledMap;

namespace Codigo.Tests
{
    [TestClass]
    public class WorldObjectTest
    {
        [TestMethod]
        public void CheckDamage()
        {
           WorldObject wo = new WorldObject();
           wo.TakeDamage(80);
           Assert.IsTrue( wo.GetHealth()> 0);
        }
    }
}
