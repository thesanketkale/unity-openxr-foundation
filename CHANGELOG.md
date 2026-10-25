# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-10-25

### Added
- **Core Subsystems (`Foundation.Core`):**
  - `XrBootstrap`: XR display subsystem readiness coordinator and tracking origin lifecycle manager.
  - `XrServiceLocator`: Non-allocating, thread-safe runtime service container for decoupled XR systems.
  - `XrFoundationSettings`: ScriptableObject holding comfort modes, haptic intensity, and tracking origin configurations.
  - `PoseMath`: Static math utility methods for non-allocating planar projections, 1D axis constraints, angle wrapping, and hysteresis calculations.
  - Strongly-typed lightweight event structs (`TrackingStateChangedEvent`, `InputModalityChangedEvent`, `InteractableValueChangedEvent`).
- **Input Subsystem (`Foundation.Input`):**
  - Unified input source contract `IXrInputSource` abstracting 6-DoF motion controllers and optical hand tracking.
  - `XrInputManager` with automatic handover, 250 ms hysteresis debounce, and input mode overrides (`Auto`, `ControllerOnly`, `HandsOnly`).
  - `XrControllerInputSource`: Input System Action Map implementation for standard VR controllers.
  - `XrHandTrackingInputSource`: Subsystem wrapper for 26-joint OpenXR optical hand tracking with normalized pinch and grip calculations.
- **Physical Interactables (`Foundation.Interactables`):**
  - Base class `XrBasePhysicalInteractable` deriving from `XRBaseInteractable` exposing `Normalized` (0..1) and `ValueChanged` events.
  - `XrPushButton`: Depth depression poke button with hysteresis click/release thresholds.
  - `XrLever`: Analytical rotation projection around pivot axis eliminating `HingeJoint` physics jitter.
  - `XrDial`: Continuous 360° or clamped rotary knob with optional stepped detents.
  - `XrSlider`: 1D axis line segment projection with customizable snap notches.
  - `XrDoor`: Kinematic hinged door with physical latch mechanism.
  - `XrSocket`: Snapping socket with holographic ghost material preview.
  - `XrPhysicsGrabInteractable`: Rigidbody grab with velocity curve throw prediction and two-handed manipulation.
- **Locomotion & UI (`Foundation.Locomotion`, `Foundation.UI`):**
  - `XrLocomotionManager`: Teleportation raycasts, snap turning, smooth turning, and tunneling comfort vignette.
  - `XrWorldSpaceCanvas`: Automated Canvas Scaler and BoxCollider setup for VR ray and poke interactions.
  - `XrBillboardLabel`: Floating label facing active XR camera with distance scaling compensation.
- **Scenes:**
  - `Bootstrap.unity`: Minimal XR engine startup scene.
  - `InteractionPlayground.unity`: Complete interactive VR demonstration scene with table, floor, and all interactable primitives.
- **Editor Tooling (`Foundation.Editor`):**
  - `XrProjectValidator`: Declarative validator reading `xr-requirements.json` with Editor GUI (`Tools > XR Foundation > Validate Project`) and batchmode CLI.
  - `BuildCommands`: Headless automated compilation runners (`Foundation.Editor.BuildCommands.Android` and `.Windows`) with Git tag versioning and keystore configuration.
- **Automated Test Suites:**
  - `XrProjectValidatorTests`: Unit tests proving validator detects seeded misconfigurations (wrong build target, Mono backend, missing ARM64, Gamma color space, invalid min SDK, missing feature groups).
  - `InteractableMathTests`: Mathematical tests for lever angles, dial wrapping, slider 1D projection, and door latches.
  - `PoseMathTests`: Unit tests for plane projection, axis clamping, hysteresis, and angle normalization.
  - `Foundation.StandaloneTests`: Standalone .NET 8 unit test project executable without Unity Editor in CI.
- **CI/CD & Governance:**
  - GitHub Actions CI workflow (`ci.yml`) with top-level zero permissions, SHA-pinned actions, fast meta/YAML linting, and standalone test runner.
  - Automated release packaging workflow (`release.yml`) on `v*.*.*` tags.
  - Security policy (`SECURITY.md`), Code of Conduct (`CODE_OF_CONDUCT.md`), Contributing guide (`CONTRIBUTING.md`), and Dependabot configuration (`dependabot.yml`).
  - Clean-room audit tools (`tools/guard.ps1`, `tools/guard.sh`) and local verification runners (`scripts/verify.ps1`, `scripts/verify.sh`).
