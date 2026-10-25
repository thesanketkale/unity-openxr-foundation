# ADR-0003: Unified Input Abstraction for Controllers and Hand Tracking

## Context
Modern OpenXR headsets seamlessly support two distinct interaction modalities: 6-DoF motion controllers and optical hand tracking. In many applications, hand tracking and controllers are implemented as completely separate hierarchies of game objects and interactors, leading to duplicate event logic, race conditions during transitions, and abrupt UI mode switches.

## Decision
We introduce a unified input abstraction via `IXrInputSource` managed by `XrInputManager`:
1. **Normalized Input Contract:** Both controllers and hands report a common contract:
   - `Handedness`: Left or Right.
   - `Pose`: 3D position and orientation.
   - `PinchStrength`: Float from `0.0f` to `1.0f` (index finger pinch or trigger pull).
   - `GripStrength`: Float from `0.0f` to `1.0f` (fist curl or controller grip button).
   - `TrackingConfidence`: Low, High, or Untracked.
2. **Automatic Handover with Hysteresis:** The `XrInputManager` automatically switches active modalities when tracking confidence shifts, utilizing a 250 ms debounce delay to prevent rapid mode flickering when controllers enter or exit tracking volume.
3. **User Override:** Developers and end users can lock interaction mode to `Auto`, `ControllerOnly`, or `HandsOnly`.

## Alternatives Considered
1. **Parallel Interactors:** Running both Controller Interactors and Hand Interactors simultaneously. Rejected because it doubles physics raycast and sphere overlap queries every frame and causes dual-hover visual glitches on interactables.
2. **Hard Restart on Mode Change:** Reloading scenes or rebuilding the XR rig on modality swap. Rejected due to jarring VR user experience.

## Consequences
- **Positive:** Single interactor pipeline handles both hands and controllers; zero duplicate interactable code; smooth user experience when picking up or putting down physical controllers.
- **Negative:** Hand gestures that have no direct controller analogue (e.g. finger guns) must be queried via extended interfaces.
