using System;
using System.IO;
using System.Numerics;
using System.Text.Json;
using Xunit;

namespace Foundation.StandaloneTests
{
    public class StandaloneMathAndValidatorTests
    {
        #region Pose Math Verification
        [Fact]
        public void ProjectPointOnPlane_ZerosZInXYPlane()
        {
            Vector3 point = new Vector3(3f, 4f, 10f);
            Vector3 planeOrigin = Vector3.Zero;
            Vector3 planeNormal = Vector3.UnitZ;

            Vector3 diff = point - planeOrigin;
            float distance = Vector3.Dot(diff, planeNormal);
            Vector3 projected = point - (planeNormal * distance);

            Assert.Equal(3f, projected.X);
            Assert.Equal(4f, projected.Y);
            Assert.Equal(0f, projected.Z);
        }

        [Fact]
        public void ProjectPointOnAxis_CorrectlyNormalizesAndClamps()
        {
            Vector3 point = new Vector3(5f, 0f, 0f);
            Vector3 axisOrigin = Vector3.Zero;
            Vector3 axisDirection = Vector3.Normalize(Vector3.UnitX);
            float min = 0f;
            float max = 2f;

            float projectedDistance = Vector3.Dot(point - axisOrigin, axisDirection);
            float clampedDistance = Math.Clamp(projectedDistance, min, max);
            float normalized = (clampedDistance - min) / (max - min);

            Assert.Equal(1.0f, normalized);
            Assert.Equal(2.0f, clampedDistance);
        }

        [Fact]
        public void CalculateHysteresis_PreventsContactBounce()
        {
            float upper = 0.80f;
            float lower = 0.40f;

            // Below upper while unpressed -> stays unpressed
            bool pressed = CalculateHysteresis(0.75f, false, upper, lower);
            Assert.False(pressed);

            // Reaches upper -> clicks pressed
            pressed = CalculateHysteresis(0.85f, false, upper, lower);
            Assert.True(pressed);

            // Relieved to 0.50 (above lower) -> stays pressed
            pressed = CalculateHysteresis(0.50f, true, upper, lower);
            Assert.True(pressed);

            // Relieved to 0.35 (below lower) -> resets
            pressed = CalculateHysteresis(0.35f, true, upper, lower);
            Assert.False(pressed);
        }

        private static bool CalculateHysteresis(float val, bool wasActive, float upper, float lower)
        {
            return wasActive ? val >= lower : val >= upper;
        }

        [Fact]
        public void WrapAngle360_HandlesOverAndUnderFlow()
        {
            Assert.Equal(350f, Wrap360(-10f));
            Assert.Equal(45f, Wrap360(405f));
            Assert.Equal(180f, Wrap360(180f));
        }

        private static float Wrap360(float angle)
        {
            angle %= 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }
        #endregion

        #region Declarative Requirements Verification
        [Fact]
        public void XrRequirementsJson_ExistsAndHasValidSchema()
        {
            string root = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(root, "xr-requirements.json")) && Directory.GetParent(root) != null)
            {
                root = Directory.GetParent(root)!.FullName;
            }

            string jsonPath = Path.Combine(root, "xr-requirements.json");
            Assert.True(File.Exists(jsonPath), $"xr-requirements.json must exist at {jsonPath}");

            string json = File.ReadAllText(jsonPath);
            using var doc = JsonDocument.Parse(json);
            var rootElement = doc.RootElement;

            Assert.True(rootElement.TryGetProperty("metadata", out var meta));
            Assert.True(meta.TryGetProperty("sourceUrl", out var sourceUrl));
            Assert.StartsWith("https://developers.meta.com/", sourceUrl.GetString());

            Assert.True(rootElement.TryGetProperty("requirements", out var req));
            Assert.Equal("Android", req.GetProperty("targetBuildTarget").GetString());
            Assert.Equal("IL2CPP", req.GetProperty("requiredScriptingBackend").GetString());
            Assert.Equal("Linear", req.GetProperty("requiredColorSpace").GetString());
            Assert.Equal(32, req.GetProperty("minimumAndroidApiLevel").GetInt32());
            Assert.Equal(34, req.GetProperty("targetAndroidApiLevel").GetInt32());
        }

        [Theory]
        [InlineData("Windows", "IL2CPP", "ARM64", "Linear", 32, false, "BuildTarget")]
        [InlineData("Android", "Mono", "ARM64", "Linear", 32, false, "ScriptingBackend")]
        [InlineData("Android", "IL2CPP", "ARMv7", "Linear", 32, false, "TargetArchitecture")]
        [InlineData("Android", "IL2CPP", "ARM64", "Gamma", 32, false, "ColorSpace")]
        [InlineData("Android", "IL2CPP", "ARM64", "Linear", 28, false, "MinimumApiLevel")]
        [InlineData("Android", "IL2CPP", "ARM64", "Linear", 34, true, null)]
        public void SeededMisconfigurations_AreDetectedCorrectly(
            string target,
            string backend,
            string arch,
            string colorSpace,
            int minSdk,
            bool expectedValid,
            string? expectedFailingRule)
        {
            string? failedRule = null;

            if (target != "Android") failedRule = "BuildTarget";
            else if (backend != "IL2CPP") failedRule = "ScriptingBackend";
            else if (arch != "ARM64") failedRule = "TargetArchitecture";
            else if (colorSpace != "Linear") failedRule = "ColorSpace";
            else if (minSdk < 32) failedRule = "MinimumApiLevel";

            if (expectedValid)
            {
                Assert.Null(failedRule);
            }
            else
            {
                Assert.Equal(expectedFailingRule, failedRule);
            }
        }
        #endregion
    }
}
