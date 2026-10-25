using System;
using Foundation.Core.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Physical rotary dial computing angle from interactor pose projected onto rotation plane.
    /// Supports stepped detents or continuous rotary control without HingeJoint jitter.
    /// </summary>
    public class XrDial : XrBasePhysicalInteractable
    {
        [Header("Dial Geometry")]
        [SerializeField] private Transform m_KnobTransform;
        [SerializeField] private Vector3 m_RotationAxis = Vector3.up;

        [Header("Angular Limits")]
        [SerializeField] private bool m_IsContinuous360 = false;
        [SerializeField] private float m_MinAngleDegrees = 0f;
        [SerializeField] private float m_MaxAngleDegrees = 270f;

        [Header("Detents / Stepping")]
        [SerializeField] private bool m_UseDetents = false;
        [SerializeField] private int m_DetentCount = 10;

        private float m_CurrentAngleDegrees;
        private Vector3 m_PreviousInteractorVector;
        private bool m_HasPreviousPose;

        public float CurrentAngleDegrees => m_CurrentAngleDegrees;
        public bool IsContinuous360 => m_IsContinuous360;

        public event Action<int> OnDetentReached;

        protected override void Awake()
        {
            base.Awake();
            m_CurrentAngleDegrees = Mathf.Lerp(m_MinAngleDegrees, m_MaxAngleDegrees, Normalized);
            UpdateKnobRotation();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            m_HasPreviousPose = false;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            m_HasPreviousPose = false;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && interactorsSelecting.Count > 0)
            {
                UpdateDialRotation(interactorsSelecting[0]);
            }
        }

        private void UpdateDialRotation(IXRSelectInteractor interactor)
        {
            Vector3 interactorWorldPos = interactor.transform.position;
            Vector3 dialPivot = transform.position;
            Vector3 worldRotationAxis = transform.TransformDirection(m_RotationAxis.normalized);

            Vector3 currentVector = interactorWorldPos - dialPivot;
            Vector3 projectedVector = PoseMath.ProjectPointOnPlane(currentVector, Vector3.zero, worldRotationAxis);

            if (projectedVector.sqrMagnitude < 0.0001f)
            {
                return;
            }

            if (!m_HasPreviousPose)
            {
                m_PreviousInteractorVector = projectedVector;
                m_HasPreviousPose = true;
                return;
            }

            float angleDelta = Vector3.SignedAngle(m_PreviousInteractorVector, projectedVector, worldRotationAxis);
            m_PreviousInteractorVector = projectedVector;

            float newAngle = m_CurrentAngleDegrees + angleDelta;

            if (m_IsContinuous360)
            {
                newAngle = PoseMath.WrapAngle360(newAngle);
                m_CurrentAngleDegrees = newAngle;
                SetNormalizedValue(newAngle / 360f);
            }
            else
            {
                float clampedAngle = Mathf.Clamp(newAngle, m_MinAngleDegrees, m_MaxAngleDegrees);
                m_CurrentAngleDegrees = clampedAngle;
                float normalized = PoseMath.NormalizeAngle(clampedAngle, m_MinAngleDegrees, m_MaxAngleDegrees);
                SetNormalizedValue(normalized);
            }

            if (m_UseDetents && m_DetentCount > 1)
            {
                int currentDetent = Mathf.RoundToInt(Normalized * (m_DetentCount - 1));
                OnDetentReached?.Invoke(currentDetent);
            }

            UpdateKnobRotation();
        }

        private void UpdateKnobRotation()
        {
            if (m_KnobTransform != null)
            {
                m_KnobTransform.localRotation = Quaternion.AngleAxis(m_CurrentAngleDegrees, m_RotationAxis);
            }
        }
    }
}
