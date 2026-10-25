using System;
using Foundation.Core.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Linear 1D slider projecting interactor position onto an axis line segment with limits and detents.
    /// </summary>
    public class XrSlider : XrBasePhysicalInteractable
    {
        [Header("Slider Geometry")]
        [SerializeField] private Transform m_HandleTransform;
        [SerializeField] private Vector3 m_TravelAxis = Vector3.right;
        [SerializeField] private float m_TravelLength = 0.3f; // 30 cm

        [Header("Notches / Snapping")]
        [SerializeField] private bool m_EnableNotches = false;
        [SerializeField] private int m_NotchCount = 5;

        private Vector3 m_InitialLocalPosition;
        private float m_CurrentPosition;

        public float TravelLength => m_TravelLength;
        public float CurrentPosition => m_CurrentPosition;

        public event Action<int> OnNotchReached;

        protected override void Awake()
        {
            base.Awake();
            if (m_HandleTransform != null)
            {
                m_InitialLocalPosition = m_HandleTransform.localPosition;
            }
            m_CurrentPosition = Normalized * m_TravelLength;
            UpdateHandlePosition();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && interactorsSelecting.Count > 0)
            {
                UpdateSliderPosition(interactorsSelecting[0]);
            }
        }

        private void UpdateSliderPosition(IXRSelectInteractor interactor)
        {
            Vector3 interactorWorldPos = interactor.transform.position;
            Vector3 sliderOriginWorld = transform.position;
            Vector3 worldTravelAxis = transform.TransformDirection(m_TravelAxis.normalized);

            PoseMath.ProjectPointOnAxis(
                interactorWorldPos,
                sliderOriginWorld,
                worldTravelAxis,
                0f,
                m_TravelLength,
                out float normalized);

            if (m_EnableNotches && m_NotchCount > 1)
            {
                float step = 1.0f / (m_NotchCount - 1);
                normalized = Mathf.Round(normalized / step) * step;
                int notchIndex = Mathf.RoundToInt(normalized * (m_NotchCount - 1));
                OnNotchReached?.Invoke(notchIndex);
            }

            m_CurrentPosition = normalized * m_TravelLength;
            SetNormalizedValue(normalized);
            UpdateHandlePosition();
        }

        private void UpdateHandlePosition()
        {
            if (m_HandleTransform != null)
            {
                m_HandleTransform.localPosition = m_InitialLocalPosition + (m_TravelAxis.normalized * m_CurrentPosition);
            }
        }
    }
}
