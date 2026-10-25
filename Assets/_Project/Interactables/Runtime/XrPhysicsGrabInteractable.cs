using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Physics grab interactable supporting two-hand grab orientation blending,
    /// dynamic velocity smoothing, and realistic throwing physics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class XrPhysicsGrabInteractable : XRGrabInteractable
    {
        [Header("Two-Hand Manipulation")]
        [SerializeField] private bool m_EnableTwoHandGrab = true;
        [SerializeField] private Transform m_SecondaryAttachTransform;

        [Header("Throw Smoothing")]
        [SerializeField] private float m_ThrowVelocityScale = 1.2f;
        [SerializeField] private float m_ThrowAngularVelocityScale = 1.0f;
        [SerializeField] private int m_VelocitySmoothingFrameCount = 5;

        private Rigidbody m_Rigidbody;
        private Vector3[] m_RecentVelocities;
        private Vector3[] m_RecentAngularVelocities;
        private int m_VelocityIndex;

        protected override void Awake()
        {
            base.Awake();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_RecentVelocities = new Vector3[m_VelocitySmoothingFrameCount];
            m_RecentAngularVelocities = new Vector3[m_VelocitySmoothingFrameCount];
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed && isSelected)
            {
                TrackVelocities();
            }
        }

        private void TrackVelocities()
        {
            if (m_Rigidbody != null)
            {
                m_RecentVelocities[m_VelocityIndex] = m_Rigidbody.linearVelocity;
                m_RecentAngularVelocities[m_VelocityIndex] = m_Rigidbody.angularVelocity;
                m_VelocityIndex = (m_VelocityIndex + 1) % m_VelocitySmoothingFrameCount;
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (m_Rigidbody != null && !m_Rigidbody.isKinematic)
            {
                ApplySmoothedThrow();
            }
        }

        private void ApplySmoothedThrow()
        {
            Vector3 averageVelocity = Vector3.zero;
            Vector3 averageAngularVelocity = Vector3.zero;

            for (int i = 0; i < m_VelocitySmoothingFrameCount; i++)
            {
                averageVelocity += m_RecentVelocities[i];
                averageAngularVelocity += m_RecentAngularVelocities[i];
            }

            averageVelocity /= m_VelocitySmoothingFrameCount;
            averageAngularVelocity /= m_VelocitySmoothingFrameCount;

            m_Rigidbody.linearVelocity = averageVelocity * m_ThrowVelocityScale;
            m_Rigidbody.angularVelocity = averageAngularVelocity * m_ThrowAngularVelocityScale;
        }
    }
}
