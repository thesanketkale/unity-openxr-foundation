using System;
using Foundation.Core.Events;
using UnityEngine;

namespace Foundation.Input
{
    /// <summary>
    /// Common contract for XR input devices (6-DoF controllers and optical hand tracking).
    /// </summary>
    public interface IXrInputSource
    {
        XrHandedness Handedness { get; }
        XrInputModality Modality { get; }
        XrTrackingConfidence TrackingConfidence { get; }
        bool IsTracked { get; }
        Pose RootPose { get; }
        Pose PointerPose { get; }
        float TriggerValue { get; }
        float GripValue { get; }
        bool IsTriggerPressed { get; }
        bool IsGripPressed { get; }

        event Action<IXrInputSource, XrTrackingConfidence> OnTrackingConfidenceChanged;

        void SendHapticFeedback(float amplitude, float duration);
    }
}
