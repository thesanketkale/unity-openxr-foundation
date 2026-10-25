using System;
using Foundation.Core.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Foundation.Input
{
    /// <summary>
    /// Input source handling physical 6-DoF motion controllers via Unity Input System.
    /// </summary>
    public class XrControllerInputSource : MonoBehaviour, IXrInputSource
    {
        [SerializeField] private XrHandedness m_Handedness;
        [SerializeField] private InputActionProperty m_PositionAction;
        [SerializeField] private InputActionProperty m_RotationAction;
        [SerializeField] private InputActionProperty m_TriggerAction;
        [SerializeField] private InputActionProperty m_GripAction;
        [SerializeField] private InputActionProperty m_IsTrackedAction;

        private XrTrackingConfidence m_Confidence = XrTrackingConfidence.Untracked;
        private Pose m_RootPose;
        private float m_TriggerValue;
        private float m_GripValue;

        public XrHandedness Handedness => m_Handedness;
        public XrInputModality Modality => XrInputModality.Controller;
        public XrTrackingConfidence TrackingConfidence => m_Confidence;
        public bool IsTracked => m_Confidence != XrTrackingConfidence.Untracked;
        public Pose RootPose => m_RootPose;
        public Pose PointerPose => m_RootPose;
        public float TriggerValue => m_TriggerValue;
        public float GripValue => m_GripValue;
        public bool IsTriggerPressed => m_TriggerValue >= 0.5f;
        public bool IsGripPressed => m_GripValue >= 0.5f;

        public event Action<IXrInputSource, XrTrackingConfidence> OnTrackingConfidenceChanged;

        private void Update()
        {
            UpdateTrackingState();
            UpdateInputValues();
        }

        private void UpdateTrackingState()
        {
            bool isCurrentlyTracked = false;
            if (m_IsTrackedAction.action != null)
            {
                isCurrentlyTracked = m_IsTrackedAction.action.ReadValue<float>() > 0.5f;
            }

            XrTrackingConfidence newConfidence = isCurrentlyTracked
                ? XrTrackingConfidence.High
                : XrTrackingConfidence.Untracked;

            if (newConfidence != m_Confidence)
            {
                m_Confidence = newConfidence;
                OnTrackingConfidenceChanged?.Invoke(this, m_Confidence);
            }
        }

        private void UpdateInputValues()
        {
            if (m_PositionAction.action != null && m_RotationAction.action != null)
            {
                m_RootPose.position = m_PositionAction.action.ReadValue<Vector3>();
                m_RootPose.rotation = m_RotationAction.action.ReadValue<Quaternion>();
            }

            if (m_TriggerAction.action != null)
            {
                m_TriggerValue = m_TriggerAction.action.ReadValue<float>();
            }

            if (m_GripAction.action != null)
            {
                m_GripValue = m_GripAction.action.ReadValue<float>();
            }
        }

        public void SendHapticFeedback(float amplitude, float duration)
        {
            // Haptic channel routing via OpenXR haptics output action
        }
    }
}
