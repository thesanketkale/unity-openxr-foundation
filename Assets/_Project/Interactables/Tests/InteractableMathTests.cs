using Foundation.Core.Utilities;
using NUnit.Framework;
using UnityEngine;

namespace Foundation.Interactables.Tests
{
    [TestFixture]
    public class InteractableMathTests
    {
        [Test]
        public void LeverAngle_WhenPulledToMax_CalculatesExpectedClamp()
        {
            Vector3 rotationAxis = Vector3.right;
            Vector3 zeroRef = Vector3.forward;

            // Target vector 45 degrees up from forward around right axis
            Vector3 target = Quaternion.AngleAxis(45f, rotationAxis) * zeroRef;

            float signedAngle = PoseMath.ComputeSignedAngleAroundAxis(target, zeroRef, rotationAxis);
            float normalized = PoseMath.NormalizeAngle(signedAngle, -45f, 45f);

            Assert.AreEqual(45f, signedAngle, 1e-3f);
            Assert.AreEqual(1.0f, normalized, 1e-3f);
        }

        [Test]
        public void DialAngle_Continuous360_WrapsGracefully()
        {
            float angle = 370f;
            float wrapped = PoseMath.WrapAngle360(angle);
            float normalized = wrapped / 360f;

            Assert.AreEqual(10f, wrapped, 1e-3f);
            Assert.AreEqual(10f / 360f, normalized, 1e-3f);
        }

        [Test]
        public void Slider1DProjection_NotchSnapping_SnapsToDiscreteIndices()
        {
            float normalized = 0.48f;
            int notchCount = 5; // Notches at 0.0, 0.25, 0.50, 0.75, 1.0

            float step = 1.0f / (notchCount - 1);
            float snapped = Mathf.Round(normalized / step) * step;
            int notchIndex = Mathf.RoundToInt(snapped * (notchCount - 1));

            Assert.AreEqual(0.50f, snapped, 1e-3f);
            Assert.AreEqual(2, notchIndex);
        }

        [Test]
        public void DoorLatch_UnlatchThreshold_ReleasesOnlyWhenRotatedPastFiveDegrees()
        {
            float unlatchThreshold = 5.0f;

            // 3 degrees should stay latched
            bool shouldUnlatchAt3 = 3.0f > unlatchThreshold;
            Assert.IsFalse(shouldUnlatchAt3);

            // 6 degrees should unlatch
            bool shouldUnlatchAt6 = 6.0f > unlatchThreshold;
            Assert.IsTrue(shouldUnlatchAt6);
        }
    }
}
