using System;
using Foundation.Core.Events;
using UnityEngine;

namespace Foundation.Input
{
    /// <summary>
    /// Input source handling optical hand tracking joints, pinch gestures, and ray poses.
    /// </summary>
    public class XrHandTrackingInputSource : MonoBehaviour, IXrInputSource
    {
        [SerializeField] private XrHandedness m_Handedness;
        [SerializeField] private float m_PinchThreshold = 0.7f;
        [SerializeField] private float m_GripThreshold = 0.7f;

        private XrTrackingConfidence m_Confidence = XrTrackingConfidence.Untracked;
        private Pose m_RootPose;
        private Pose m_PointerPose;
        private float m_PinchStrength;
        private float m_GripStrength;

        public XrHandedness Handedness => m_Handedness;
        public XrInputModality Modality => XrInputModality.HandTracking;
        public XrTrackingConfidence TrackingConfidence => m_Confidence;
        public bool IsTracked => m_Confidence != XrTrackingConfidence.Untracked;
        public Pose RootPose => m_RootPose;
        public Pose PointerPose => m_PointerPose;
        public float TriggerValue => m_PinchStrength;
        public float GripValue => m_GripStrength;
        public bool IsTriggerPressed => m_PinchStrength >= m_PinchThreshold;
        public bool IsGripPressed => m_GripStrength >= m_GripThreshold;

        public event Action<IXrInputSource, XrTrackingConfidence> OnTrackingConfidenceChanged;

        public void UpdateHandData(Pose wristPose, Pose pointerPose, float pinchStrength, float gripStrength, XrTrackingConfidence confidence)
        {
            m_RootPose = wristPose;
            m_PointerPose = pointerPose;
            m_PinchStrength = Mathf.Clamp01(pinchStrength);
            m_GripStrength = Mathf.Clamp01(gripStrength);

            if (m_Confidence != confidence)
            {
                m_Confidence = confidence;
                OnTrackingConfidenceChanged?.Invoke(this, m_Confidence);
            }
        }

        public void SendHapticFeedback(float amplitude, float duration)
        {
            // Hand tracking devices typically lack physical actuators; optional audio or visual pulse
        }
    }
}
