using Foundation.Core.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Foundation.Core.Tests
{
    [TestFixture]
    public class PoseMathTests
    {
        [Test]
        public void ProjectPointOnPlane_WithXyPlane_ZerosZCoordinate()
        {
            Vector3 point = new Vector3(2f, 3f, 5f);
            Vector3 origin = Vector3.zero;
            Vector3 normal = Vector3.forward; // Normal along Z

            Vector3 projected = PoseMath.ProjectPointOnPlane(point, origin, normal);

            Assert.AreEqual(2f, projected.x, 1e-4f);
            Assert.AreEqual(3f, projected.y, 1e-4f);
            Assert.AreEqual(0f, projected.z, 1e-4f);
        }

        [Test]
        public void ProjectPointOnAxis_ClampsWithinMinMaxRange()
        {
            Vector3 point = new Vector3(5f, 0f, 0f);
            Vector3 origin = Vector3.zero;
            Vector3 direction = Vector3.right;
            float min = 0f;
            float max = 2f;

            Vector3 projected = PoseMath.ProjectPointOnAxis(point, origin, direction, min, max, out float normalized);

            Assert.AreEqual(2f, projected.x, 1e-4f);
            Assert.AreEqual(1f, normalized, 1e-4f);
        }

        [Test]
        public void NormalizeAngle_CorrectlyMapsAngleToZeroOne()
        {
            float normalizedZero = PoseMath.NormalizeAngle(0f, -45f, 45f);
            float normalizedMin = PoseMath.NormalizeAngle(-45f, -45f, 45f);
            float normalizedMax = PoseMath.NormalizeAngle(45f, -45f, 45f);

            Assert.AreEqual(0.5f, normalizedZero, 1e-4f);
            Assert.AreEqual(0.0f, normalizedMin, 1e-4f);
            Assert.AreEqual(1.0f, normalizedMax, 1e-4f);
        }

        [Test]
        public void WrapAngle360_HandlesNegativeAndOverflow()
        {
            Assert.AreEqual(350f, PoseMath.WrapAngle360(-10f), 1e-4f);
            Assert.AreEqual(45f, PoseMath.WrapAngle360(405f), 1e-4f);
            Assert.AreEqual(180f, PoseMath.WrapAngle360(180f), 1e-4f);
        }

        [Test]
        public void CalculateHysteresis_PreventsChatter()
        {
            float upper = 0.8f;
            float lower = 0.4f;

            // Initially inactive: 0.7 does not trigger
            bool active = PoseMath.CalculateHysteresis(0.7f, false, upper, lower);
            Assert.IsFalse(active);

            // Crosses upper threshold 0.85 -> triggers
            active = PoseMath.CalculateHysteresis(0.85f, false, upper, lower);
            Assert.IsTrue(active);

            // Was active, drops to 0.6 (above lower) -> stays active
            active = PoseMath.CalculateHysteresis(0.6f, true, upper, lower);
            Assert.IsTrue(active);

            // Was active, drops to 0.3 (below lower) -> deactivates
            active = PoseMath.CalculateHysteresis(0.3f, true, upper, lower);
            Assert.IsFalse(active);
        }
    }
}
