using System;
using UnityEngine;

namespace Foundation.UI
{
    /// <summary>
    /// Floating billboard label that continuously faces the active XR Camera rig
    /// with optional distance scale compensation to maintain legibility.
    /// </summary>
    public class XrBillboardLabel : MonoBehaviour
    {
        [Header("Billboard Settings")]
        [SerializeField] private bool m_LockXAxis = false;
        [SerializeField] private bool m_LockYAxis = false;
        [SerializeField] private bool m_LockZAxis = false;
        [SerializeField] private bool m_FlipForward = false;

        [Header("Distance Scaling")]
        [SerializeField] private bool m_CompensateDistanceScale = false;
        [SerializeField] private float m_ReferenceDistance = 1.5f;
        [SerializeField] private float m_MinScaleMultiplier = 0.5f;
        [SerializeField] private float m_MaxScaleMultiplier = 2.0f;

        private Transform m_CameraTransform;
        private Vector3 m_InitialLocalScale;

        private void Awake()
        {
            m_InitialLocalScale = transform.localScale;
            FindCamera();
        }

        private void OnEnable()
        {
            FindCamera();
        }

        private void FindCamera()
        {
            if (Camera.main != null)
            {
                m_CameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (m_CameraTransform == null)
            {
                FindCamera();
                if (m_CameraTransform == null) return;
            }

            OrientTowardCamera();

            if (m_CompensateDistanceScale)
            {
                ApplyDistanceScale();
            }
        }

        private void OrientTowardCamera()
        {
            Vector3 lookDir = m_CameraTransform.position - transform.position;
            if (m_FlipForward)
            {
                lookDir = -lookDir;
            }

            if (m_LockXAxis) lookDir.x = 0f;
            if (m_LockYAxis) lookDir.y = 0f;
            if (m_LockZAxis) lookDir.z = 0f;

            if (lookDir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }
        }

        private void ApplyDistanceScale()
        {
            float distance = Vector3.Distance(transform.position, m_CameraTransform.position);
            float scaleFactor = distance / m_ReferenceDistance;
            scaleFactor = Mathf.Clamp(scaleFactor, m_MinScaleMultiplier, m_MaxScaleMultiplier);
            transform.localScale = m_InitialLocalScale * scaleFactor;
        }
    }
}
