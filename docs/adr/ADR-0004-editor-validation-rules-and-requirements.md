# ADR-0004: Declarative Offline Project Validation

## Context
Deploying Unity applications to standalone Android OpenXR hardware (such as Meta Quest) involves dozens of interconnected project settings across Player Settings, Quality Settings, OpenXR Feature Groups, and Android Manifest declarations. If any setting is misconfigured (e.g. Gamma color space, Mono scripting backend, missing ARM64, or disabled OpenXR feature groups), builds will fail or crash at launch with cryptic logcat errors.

## Decision
We implement a declarative project validator `XrProjectValidator` backed by `xr-requirements.json`:
1. **Externalised Requirements:** Thresholds and mandatory settings are declared in `xr-requirements.json`, citing authoritative documentation URLs (Meta Horizon OS developer docs and Unity OpenXR manual) and retrieval timestamps.
2. **Dual Execution Mode:**
   - In Editor GUI: Accessible via `Tools > XR Foundation > Validate Project`, displaying clear pass/fail badges with fix recommendations.
   - In Headless Batchmode: Accessible via `-executeMethod Foundation.Editor.XrProjectValidator.ValidateBatch`, terminating with exit code `0` on success and `1` on failure for CI pipelines.
3. **Seeded Testability:** The validator's rule engine evaluates decoupled setting providers, allowing unit tests to simulate misconfigurations (e.g., Mono scripting backend, missing ARM64, invalid API level) without modifying the actual live project settings.

## Alternatives Considered
1. **Unity Project Validation Package (`com.unity.xr.core-utils` validation):** Good for basic Editor dialogs, but awkward to drive via headless batchmode scripts and non-trivial to unit-test with seeded failures.
2. **Hardcoded C# Validation Rules:** Violates clean configuration principles; changes to platform requirements require code re-compilation.

## Consequences
- **Positive:** Catch configuration drift immediately in local development and automated CI; fully testable via unit tests; self-documenting requirements file.
- **Negative:** Requirements file must be maintained as platform specifications advance.
