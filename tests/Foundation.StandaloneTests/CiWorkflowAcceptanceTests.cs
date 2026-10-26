using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace Foundation.StandaloneTests
{
    public class CiWorkflowAcceptanceTests
    {
        private static string GetWorkflowContent()
        {
            string root = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(root, ".github", "workflows", "ci.yml")) && Directory.GetParent(root) != null)
            {
                root = Directory.GetParent(root)!.FullName;
            }

            string workflowPath = Path.Combine(root, ".github", "workflows", "ci.yml");
            Assert.True(File.Exists(workflowPath), $"ci.yml must exist at {workflowPath}");
            return File.ReadAllText(workflowPath);
        }

        [Fact]
        public void AC_1_AllJobsUseUbuntu2404()
        {
            string yaml = GetWorkflowContent();

            // All jobs must pin ubuntu-24.04 and not ubuntu-latest
            Assert.DoesNotContain("runs-on: ubuntu-latest", yaml);

            var matches = Regex.Matches(yaml, @"runs-on:\s*ubuntu-24\.04");
            Assert.True(matches.Count >= 4, $"Expected at least 4 jobs pinned to ubuntu-24.04, found {matches.Count}");
        }

        [Fact]
        public void AC_2_ActionsPinnedToNode24Shas()
        {
            string yaml = GetWorkflowContent();

            // Checkout action v7.0.1
            Assert.Contains("actions/checkout@3d3c42e5aac5ba805825da76410c181273ba90b1", yaml);
            Assert.Contains("# v7.0.1", yaml);
            Assert.DoesNotContain("11bd71901bbe5b1630ceea73d27597364c9af683", yaml);

            // Setup dotnet action v6.0.0
            Assert.Contains("actions/setup-dotnet@a98b56852c35b8e3190ac28c8c2271da59106c68", yaml);
            Assert.Contains("# v6.0.0", yaml);
            Assert.DoesNotContain("3951f0dfe7a07e2313ec93c75700083e2005cbab", yaml);

            // Gitleaks action v3.0.0
            Assert.Contains("gitleaks/gitleaks-action@e0c47f4f8be36e29cdc102c57e68cb5cbf0e8d1e", yaml);
            Assert.Contains("# v3.0.0", yaml);
            Assert.DoesNotContain("ff98106e4c7b2bc287b24eaf42907196329070c7", yaml);
        }

        [Fact]
        public void AC_3_LintJobGeneratesStepSummary()
        {
            string yaml = GetWorkflowContent();

            // The lint-and-validate job must write markdown step summary
            int lintJobIndex = yaml.IndexOf("lint-and-validate:", StringComparison.Ordinal);
            Assert.True(lintJobIndex >= 0, "lint-and-validate job must exist");

            int nextJobIndex = yaml.IndexOf("tests-and-quality:", lintJobIndex, StringComparison.Ordinal);
            string lintSection = nextJobIndex > 0 ? yaml.Substring(lintJobIndex, nextJobIndex - lintJobIndex) : yaml.Substring(lintJobIndex);

            Assert.Contains("$GITHUB_STEP_SUMMARY", lintSection);
            Assert.Contains("Verification Summary", lintSection);
        }

        [Fact]
        public void AC_4_TestsJobGeneratesStepSummary()
        {
            string yaml = GetWorkflowContent();

            int testJobIndex = yaml.IndexOf("tests-and-quality:", StringComparison.Ordinal);
            Assert.True(testJobIndex >= 0, "tests-and-quality job must exist");

            int nextJobIndex = yaml.IndexOf("security-scan:", testJobIndex, StringComparison.Ordinal);
            string testSection = nextJobIndex > 0 ? yaml.Substring(testJobIndex, nextJobIndex - testJobIndex) : yaml.Substring(testJobIndex);

            Assert.Contains("$GITHUB_STEP_SUMMARY", testSection);
            Assert.Contains("Test Suite Summary", testSection);
        }

        [Fact]
        public void AC_5_SecurityJobGeneratesStepSummary()
        {
            string yaml = GetWorkflowContent();

            int secJobIndex = yaml.IndexOf("security-scan:", StringComparison.Ordinal);
            Assert.True(secJobIndex >= 0, "security-scan job must exist");

            int nextJobIndex = yaml.IndexOf("unity-xr-build:", secJobIndex, StringComparison.Ordinal);
            string secSection = nextJobIndex > 0 ? yaml.Substring(secJobIndex, nextJobIndex - secJobIndex) : yaml.Substring(secJobIndex);

            Assert.Contains("$GITHUB_STEP_SUMMARY", secSection);
            Assert.Contains("Security Audit Summary", secSection);
        }

        [Fact]
        public void AC_6_BuildJobGeneratesStepSummary()
        {
            string yaml = GetWorkflowContent();

            int buildJobIndex = yaml.IndexOf("unity-xr-build:", StringComparison.Ordinal);
            Assert.True(buildJobIndex >= 0, "unity-xr-build job must exist");

            string buildSection = yaml.Substring(buildJobIndex);

            Assert.Contains("$GITHUB_STEP_SUMMARY", buildSection);
            Assert.Contains("Unity Standalone Build", buildSection);
        }

        [Fact(Skip = "pending AC-7 (step 3)")]
        public void AC_7_WorkflowAdheresToYamlConventions()
        {
            string root = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(root, ".github", "workflows", "ci.yml")) && Directory.GetParent(root) != null)
            {
                root = Directory.GetParent(root)!.FullName;
            }

            string workflowPath = Path.Combine(root, ".github", "workflows", "ci.yml");
            string yaml = File.ReadAllText(workflowPath);

            Assert.DoesNotContain("\t", yaml); // No tab indentation in YAML
            Assert.True(yaml.Length > 0);
        }

        [Fact]
        public void AC_8_WorkflowGuardrailsAndPermissions()
        {
            string yaml = GetWorkflowContent();

            // Top-level zero permissions
            Assert.Matches(@"(?m)^permissions:\s*\{\}", yaml);

            // Concurrency configured
            Assert.Contains("concurrency:", yaml);
            Assert.Contains("cancel-in-progress:", yaml);

            // Explicit timeouts on all jobs
            var timeoutMatches = Regex.Matches(yaml, @"timeout-minutes:\s*(\d+)");
            Assert.True(timeoutMatches.Count >= 4, $"Expected timeout-minutes on all 4 jobs, found {timeoutMatches.Count}");
            foreach (Match match in timeoutMatches)
            {
                int minutes = int.Parse(match.Groups[1].Value);
                Assert.True(minutes <= 45, $"Job timeout exceeds maximum allowable limit (found {minutes})");
            }
        }
    }
}
