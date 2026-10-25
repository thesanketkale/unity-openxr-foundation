using System;
using Foundation.Core.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Physical lever computing angle from interactor pose projected onto its rotation plane.
    /// Operates without HingeJoint to eliminate PhysX solver jitter in VR.
    /// </summary>
    public class XrLever : XrBasePhysicalInteractable
    {
        [Header("Lever Geometry")]
        [SerializeField] private Transform m_HandleTransform;
        [SerializeField] private Vector3 m_RotationAxis = Vector3.right;
        [SerializeField] private Vector3 m_ZeroReferenceDirection = Vector3.forward;

        [Header("Angular Limits")]
        [SerializeField] private float m_MinAngleDegrees = -45f;
        [SerializeField] private float m_MaxAngleDegrees = 45f;

        private float m_CurrentAngleDegrees;

        public float CurrentAngleDegrees => m_CurrentAngleDegrees;
        public float MinAngleDegrees => m_MinAngleDegrees;
        public float MaxAngleDegrees => m_MaxAngleDegrees;

        protected override void Awake()
        {
            base.Awake();
            m_CurrentAngleDegrees = Mathf.Lerp(m_MinAngleDegrees, m_MaxAngleDegrees, Normalized);
            UpdateHandleRotation();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && interactorsSelecting.Count > 0)
            {
                UpdateLeverAngle(interactorsSelecting[0]);
            }
        }

        private void UpdateLeverAngle(IXRSelectInteractor interactor)
        {
            Vector3 interactorWorldPos = interactor.transform.position;
            Vector3 leverPivotWorld = transform.position;
            Vector3 worldRotationAxis = transform.TransformDirection(m_RotationAxis.normalized);
            Vector3 worldRefDirection = transform.TransformDirection(m_ZeroReferenceDirection.normalized);

            Vector3 toInteractor = interactorWorldPos - leverPivotWorld;
            float signedAngle = PoseMath.ComputeSignedAngleAroundAxis(toInteractor, worldRefDirection, worldRotationAxis);

            float clampedAngle = Mathf.Clamp(signedAngle, m_MinAngleDegrees, m_MaxAngleDegrees);
            m_CurrentAngleDegrees = clampedAngle;

            float normalized = PoseMath.NormalizeAngle(clampedAngle, m_MinAngleDegrees, m_MaxAngleDegrees);
            SetNormalizedValue(normalized);

            UpdateHandleRotation();
        }

        private void UpdateHandleRotation()
        {
            if (m_HandleTransform != null)
            {
                m_HandleTransform.localRotation = Quaternion.AngleAxis(m_CurrentAngleDegrees, m_RotationAxis);
            }
        }

        public void SetAngleDirectly(float angleDegrees)
        {
            m_CurrentAngleDegrees = Mathf.Clamp(angleDegrees, m_MinAngleDegrees, m_MaxAngleDegrees);
            float normalized = PoseMath.NormalizeAngle(m_CurrentAngleDegrees, m_MinAngleDegrees, m_MaxAngleDegrees);
            SetNormalizedValue(normalized);
            UpdateHandleRotation();
        }
    }
}
