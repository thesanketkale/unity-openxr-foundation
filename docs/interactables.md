# Physical Interactables Guide

## Architecture & Base Contract

All physical interactables in `unity-openxr-starter` inherit from `XrBasePhysicalInteractable`, which extends Unity XR Interaction Toolkit's `XRBaseInteractable`.

### Core API Properties and Events

```csharp
public abstract class XrBasePhysicalInteractable : XRBaseInteractable
{
    // Normalized value of the interactable between 0.0f and 1.0f
    public float Normalized { get; }

    // UnityEvent and C# event fired when normalized value changes beyond epsilon
    public UnityEvent<float> OnValueChanged { get; }
    public event Action<float> ValueChanged;

    // Checks if an interactor is currently selecting or hovering this object
    public bool HasActiveInteractor { get; }

    // Helper methods for haptic and audio feedback
    public void PlayAudio(AudioClip clip);
    public void TriggerHaptic(IXRInteractor interactor, float intensity, float duration);
}
```

---

## Included Interactable Primitives

### 1. `XrPushButton`
- **Mechanism:** Linear depression along a configurable local axis (`Vector3.down`, `Vector3.back`).
- **Hysteresis Click:** Prevents contact chatter by using dual thresholds:
  - Upper threshold (`0.80f`): Poke depth must cross 80% to register a click.
  - Lower threshold (`0.40f`): Button must return above 40% before another click can occur.
- **Spring Return:** Kinematic interpolation returning button to resting position when released.

### 2. `XrLever`
- **Mechanism:** Rotational pose projection around `m_RotationAxis`.
- **Zero Jitter:** The interactor's grab position is projected onto the perpendicular 2D plane. `Mathf.Atan2` computes the signed angle relative to `m_ZeroReferenceDirection`. Clamped strictly between `[m_MinAngleDegrees, m_MaxAngleDegrees]`.

### 3. `XrDial`
- **Mechanism:** Continuous 360° or clamped rotary knob.
- **Detents:** Optional discrete stepping with audible/haptic feedback on reaching notches.

### 4. `XrSlider`
- **Mechanism:** Linear travel along a 1D line segment defined by `m_TravelAxis` and `m_TravelLength`.
- **Notches:** Configurable discrete snap stops (`m_EnableNotches`, `m_NotchCount`).

### 5. `XrDoor`
- **Mechanism:** Hinged swing with friction damping and a physical latch handle requirement.

### 6. `XrSocket`
- **Mechanism:** Snapping socket with holographic ghost material preview when a valid item hovers nearby.

---

## Worked Example: Adding a Custom Valve Wheel

Below is a complete, production-grade example of a custom rotating valve wheel:

```csharp
using Foundation.Core.Utilities;
using Foundation.Interactables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables.Custom
{
    /// <summary>
    /// Multi-turn valve wheel that accumulates rotations to open/close a fluid pipe.
    /// </summary>
    public class XrValveWheel : XrBasePhysicalInteractable
    {
        [Header("Valve Configuration")]
        [SerializeField] private Transform m_WheelTransform;
        [SerializeField] private Vector3 m_RotationAxis = Vector3.forward;
        [SerializeField] private float m_TotalRotationsToFullyOpen = 3.0f;

        private float m_CurrentTotalAngle;
        private Vector3 m_PreviousProjectedVector;
        private bool m_IsTracking;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            m_IsTracking = false;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && interactorsSelecting.Count > 0)
            {
                UpdateWheelRotation(interactorsSelecting[0]);
            }
        }

        private void UpdateWheelRotation(IXRSelectInteractor interactor)
        {
            Vector3 interactorWorld = interactor.transform.position;
            Vector3 pivot = transform.position;
            Vector3 worldAxis = transform.TransformDirection(m_RotationAxis.normalized);

            Vector3 currentVector = interactorWorld - pivot;
            Vector3 projected = PoseMath.ProjectPointOnPlane(currentVector, Vector3.zero, worldAxis);

            if (projected.sqrMagnitude < 0.0001f) return;

            if (!m_IsTracking)
            {
                m_PreviousProjectedVector = projected;
                m_IsTracking = true;
                return;
            }

            float deltaAngle = Vector3.SignedAngle(m_PreviousProjectedVector, projected, worldAxis);
            m_PreviousProjectedVector = projected;

            float maxTotalAngle = m_TotalRotationsToFullyOpen * 360f;
            m_CurrentTotalAngle = Mathf.Clamp(m_CurrentTotalAngle + deltaAngle, 0f, maxTotalAngle);

            float normalized = maxTotalAngle > 0f ? m_CurrentTotalAngle / maxTotalAngle : 0f;
            SetNormalizedValue(normalized);

            if (m_WheelTransform != null)
            {
                m_WheelTransform.localRotation = Quaternion.AngleAxis(m_CurrentTotalAngle, m_RotationAxis);
            }
        }
    }
}
```
