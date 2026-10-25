using System;
using Foundation.Core.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Physical door panel with hinge angle calculation and a handle latch mechanism.
    /// Operates without HingeJoint to eliminate physics solver jitter.
    /// </summary>
    public class XrDoor : XrBasePhysicalInteractable
    {
        [Header("Door Geometry")]
        [SerializeField] private Transform m_DoorPanel;
        [SerializeField] private Transform m_HingePivot;
        [SerializeField] private Vector3 m_HingeAxis = Vector3.up;
        [SerializeField] private Vector3 m_ClosedReferenceDirection = Vector3.forward;

        [Header("Hinge Limits")]
        [SerializeField] private float m_MinAngleDegrees = 0f;
        [SerializeField] private float m_MaxAngleDegrees = 90f;

        [Header("Latch Mechanism")]
        [SerializeField] private bool m_IsLatched = true;
        [SerializeField] private float m_UnlatchThresholdDegrees = 5f;
        [SerializeField] private AudioClip m_LatchClickClip;

        private float m_CurrentAngleDegrees;

        public bool IsLatched => m_IsLatched;
        public float CurrentAngleDegrees => m_CurrentAngleDegrees;

        public event Action OnDoorLatched;
        public event Action OnDoorUnlatched;

        protected override void Awake()
        {
            base.Awake();
            m_CurrentAngleDegrees = Mathf.Lerp(m_MinAngleDegrees, m_MaxAngleDegrees, Normalized);
            UpdateDoorRotation();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && interactorsSelecting.Count > 0)
            {
                UpdateDoorSwing(interactorsSelecting[0]);
            }
        }

        private void UpdateDoorSwing(IXRSelectInteractor interactor)
        {
            Vector3 interactorWorldPos = interactor.transform.position;
            Vector3 hingePivotWorld = m_HingePivot != null ? m_HingePivot.position : transform.position;
            Vector3 worldHingeAxis = transform.TransformDirection(m_HingeAxis.normalized);
            Vector3 worldClosedRef = transform.TransformDirection(m_ClosedReferenceDirection.normalized);

            Vector3 toInteractor = interactorWorldPos - hingePivotWorld;
            float signedAngle = PoseMath.ComputeSignedAngleAroundAxis(toInteractor, worldClosedRef, worldHingeAxis);

            float clampedAngle = Mathf.Clamp(signedAngle, m_MinAngleDegrees, m_MaxAngleDegrees);

            // Check latch state
            if (m_IsLatched && clampedAngle > m_UnlatchThresholdDegrees)
            {
                Unlatch();
            }
            else if (!m_IsLatched && clampedAngle <= 1.0f)
            {
                Latch();
                clampedAngle = 0f;
            }

            if (m_IsLatched)
            {
                clampedAngle = 0f;
            }

            m_CurrentAngleDegrees = clampedAngle;
            float normalized = PoseMath.NormalizeAngle(clampedAngle, m_MinAngleDegrees, m_MaxAngleDegrees);
            SetNormalizedValue(normalized);

            UpdateDoorRotation();
        }

        public void Unlatch()
        {
            if (m_IsLatched)
            {
                m_IsLatched = false;
                PlayAudio(m_LatchClickClip);
                OnDoorUnlatched?.Invoke();
            }
        }

        public void Latch()
        {
            if (!m_IsLatched)
            {
                m_IsLatched = true;
                PlayAudio(m_LatchClickClip);
                OnDoorLatched?.Invoke();
            }
        }

        private void UpdateDoorRotation()
        {
            if (m_DoorPanel != null)
            {
                m_DoorPanel.localRotation = Quaternion.AngleAxis(m_CurrentAngleDegrees, m_HingeAxis);
            }
        }
    }
}
