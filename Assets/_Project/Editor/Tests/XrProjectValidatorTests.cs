using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Foundation.Editor.Tests
{
    [TestFixture]
    public class XrProjectValidatorTests
    {
        private class MockSettingsQuery : IXrProjectSettingsQuery
        {
            public BuildTarget CurrentBuildTarget { get; set; } = BuildTarget.Android;
            public ScriptingImplementation CurrentScriptingBackend { get; set; } = ScriptingImplementation.IL2CPP;
            public AndroidArchitecture CurrentArchitecture { get; set; } = AndroidArchitecture.ARM64;
            public ColorSpace CurrentColorSpace { get; set; } = ColorSpace.Linear;
            public GraphicsDeviceType[] CurrentGraphicsApis { get; set; } = new[] { GraphicsDeviceType.Vulkan };
            public int CurrentMinSdkVersion { get; set; } = 32;
            public int CurrentTargetSdkVersion { get; set; } = 34;
            public bool HasHandTrackingFeature { get; set; } = true;
            public bool HasMetaQuestFeatureGroup { get; set; } = true;
            public bool HasTouchControllerProfile { get; set; } = true;
        }

        private const string SampleRequirementsJson = @"{
            ""requirements"": {
                ""targetBuildTarget"": ""Android"",
                ""requiredScriptingBackend"": ""IL2CPP"",
                ""requiredArchitectures"": [""ARM64""],
                ""requiredColorSpace"": ""Linear"",
                ""allowedGraphicsApis"": [""Vulkan"", ""OpenGLES3""],
                ""minimumAndroidApiLevel"": 32,
                ""targetAndroidApiLevel"": 34
            }
        }";

        [Test]
        public void Validate_WhenAllSettingsCompliant_ReturnsZeroErrors()
        {
            var mock = new MockSettingsQuery();
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            int errorCount = issues.FindAll(i => i.Severity == ValidationSeverity.Error).Count;
            Assert.AreEqual(0, errorCount, "Expected zero errors for compliant configuration.");
        }

        [Test]
        public void Validate_WhenBuildTargetIsStandalone_DetectsBuildTargetError()
        {
            var mock = new MockSettingsQuery { CurrentBuildTarget = BuildTarget.StandaloneWindows64 };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "BuildTarget");
            Assert.IsNotNull(issue, "Expected BuildTarget violation.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenScriptingBackendIsMono_DetectsScriptingBackendError()
        {
            var mock = new MockSettingsQuery { CurrentScriptingBackend = ScriptingImplementation.Mono2x };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "ScriptingBackend");
            Assert.IsNotNull(issue, "Expected ScriptingBackend violation for Mono.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenArchitectureLacksArm64_DetectsArchitectureError()
        {
            var mock = new MockSettingsQuery { CurrentArchitecture = AndroidArchitecture.ARMv7 };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "TargetArchitecture");
            Assert.IsNotNull(issue, "Expected TargetArchitecture violation when ARM64 is missing.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenColorSpaceIsGamma_DetectsColorSpaceError()
        {
            var mock = new MockSettingsQuery { CurrentColorSpace = ColorSpace.Gamma };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "ColorSpace");
            Assert.IsNotNull(issue, "Expected ColorSpace violation for Gamma.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenMinSdkVersionBelow32_DetectsApiLevelError()
        {
            var mock = new MockSettingsQuery { CurrentMinSdkVersion = 29 };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "MinimumApiLevel");
            Assert.IsNotNull(issue, "Expected MinimumApiLevel violation for API 29.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenHandTrackingFeatureMissing_DetectsFeatureError()
        {
            var mock = new MockSettingsQuery { HasHandTrackingFeature = false };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "HandTrackingFeature");
            Assert.IsNotNull(issue, "Expected HandTrackingFeature violation.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }

        [Test]
        public void Validate_WhenMetaQuestFeatureGroupMissing_DetectsMetaQuestError()
        {
            var mock = new MockSettingsQuery { HasMetaQuestFeatureGroup = false };
            List<ValidationIssue> issues = XrProjectValidator.RunValidation(mock, SampleRequirementsJson);

            ValidationIssue issue = issues.Find(i => i.RuleName == "MetaQuestFeatureGroup");
            Assert.IsNotNull(issue, "Expected MetaQuestFeatureGroup violation.");
            Assert.AreEqual(ValidationSeverity.Error, issue.Severity);
        }
    }
}
