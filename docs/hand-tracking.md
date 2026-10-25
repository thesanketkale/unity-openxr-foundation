# Optical Hand Tracking Architecture

## Overview

`unity-openxr-starter` utilizes the official Unity `com.unity.xr.hands` (1.5.0) and `com.unity.xr.openxr` packages to support high-fidelity optical hand tracking on standalone OpenXR devices.

---

## 1. Hand Tracking Subsystem Integration

Hand tracking is exposed via the Khronos OpenXR Hand Tracking Extension (`XR_EXT_hand_tracking`). The package translates native joint poses into Unity `XRHandSubsystem` joint representations without proprietary SDK locks.

### Joint Hierarchy

OpenXR provides 26 tracked joints per hand:
- `Wrist` (root anchor)
- `Palm`
- `Thumb`: Metacarpal, Proximal, Distal, Tip
- `Index`: Metacarpal, Proximal, Intermediate, Distal, Tip
- `Middle`: Metacarpal, Proximal, Intermediate, Distal, Tip
- `Ring`: Metacarpal, Proximal, Intermediate, Distal, Tip
- `Little`: Metacarpal, Proximal, Intermediate, Distal, Tip

---

## 2. Gesture Classification & Normalized Values

`Foundation.Input` provides continuous normalized metrics rather than binary gestures:

```csharp
public struct XrHandPoseData
{
    public float PinchStrength; // 0.0 (open) to 1.0 (fingertips touching)
    public float GripStrength;  // 0.0 (open palm) to 1.0 (closed fist)
    public bool IsPointerActive; // Hand raycast readiness
}
```

### Pinch Calculation
The index pinch is calculated as the Euclidean distance between `ThumbTip` and `IndexTip` normalized across individual hand size calibration:

$$\text{PinchStrength} = \text{Clamp01}\left(1.0 - \frac{d - d_{\text{min}}}{d_{\text{max}} - d_{\text{min}}}\right)$$

Where $d_{\text{min}} \approx 0.015\text{ m}$ (1.5 cm) and $d_{\text{max}} \approx 0.07\text{ m}$ (7 cm).

---

## 3. Seamless Handover Between Hands and Controllers

The `XrInputManager` automatically manages transition between physical 6-DoF motion controllers and optical hand tracking:

1. **Tracking State Query:** Every frame, `XrInputManager` checks `IXrInputSource.TrackingConfidence`.
2. **Controller Priority:** If a motion controller is detected and tracked, it is prioritized because physical buttons offer superior tactile certainty.
3. **250 ms Hysteresis Debounce:** When a user sets down controllers to use hands (or picks up controllers), the manager waits 250 ms before re-routing interactor rigs. This prevents mode vibration caused by temporary occlusion.
4. **User Override:** Applications can lock input mode to `ControllerOnly` or `HandsOnly` via `XrInputManager.DeviceMode`.
