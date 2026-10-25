using System;
using Foundation.Core;
using Foundation.Core.Utilities;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Push button with poke depression along a local axis, spring return,
    /// and hysteresis click threshold to prevent contact bounce.
    /// </summary>
    public class XrPushButton : XrBasePhysicalInteractable
    {
        [Header("Depression Configuration")]
        [SerializeField] private Transform m_MovingPart;
        [SerializeField] private Vector3 m_LocalPushDirection = Vector3.down;
        [SerializeField] private float m_MaxDepression = 0.03f; // 3 cm
        [SerializeField] private float m_SpringReturnSpeed = 10f;

        [Header("Hysteresis Thresholds")]
        [Range(0f, 1f)]
        [SerializeField] private float m_ClickThreshold = XrConstants.ButtonPokeHysteresisUpper;
        [Range(0f, 1f)]
        [SerializeField] private float m_ReleaseThreshold = XrConstants.ButtonPokeHysteresisLower;

        private Vector3 m_InitialLocalPosition;
        private bool m_IsPressed;
        private float m_CurrentDepression;

        public bool IsPressed => m_IsPressed;
        public float CurrentDepression => m_CurrentDepression;

        public event Action OnClicked;
        public event Action OnReleased;

        protected override void Awake()
        {
            base.Awake();
            if (m_MovingPart != null)
            {
                m_InitialLocalPosition = m_MovingPart.localPosition;
            }
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected && interactorsSelecting.Count > 0)
                {
                    UpdateDepressionFromInteractor(interactorsSelecting[0]);
                }
                else
                {
                    ReturnSpring();
                }
            }
        }

        private void UpdateDepressionFromInteractor(IXRSelectInteractor interactor)
        {
            if (m_MovingPart == null) return;

            Vector3 interactorPos = interactor.transform.position;
            Vector3 worldPushDir = transform.TransformDirection(m_LocalPushDirection.normalized);
            Vector3 startPos = transform.position;

            Vector3 projected = PoseMath.ProjectPointOnAxis(
                interactorPos,
                startPos,
                worldPushDir,
                0f,
                m_MaxDepression,
                out float normalizedDepression);

            m_CurrentDepression = normalizedDepression * m_MaxDepression;
            m_MovingPart.localPosition = m_InitialLocalPosition + (m_LocalPushDirection.normalized * m_CurrentDepression);
            SetNormalizedValue(normalizedDepression);

            EvaluateHysteresis(normalizedDepression, interactor);
        }

        private void ReturnSpring()
        {
            if (m_CurrentDepression > 0.0001f)
            {
                m_CurrentDepression = Mathf.MoveTowards(m_CurrentDepression, 0f, m_SpringReturnSpeed * Time.deltaTime);
                float normalized = m_MaxDepression > 0f ? m_CurrentDepression / m_MaxDepression : 0f;

                if (m_MovingPart != null)
                {
                    m_MovingPart.localPosition = m_InitialLocalPosition + (m_LocalPushDirection.normalized * m_CurrentDepression);
                }

                SetNormalizedValue(normalized);
                EvaluateHysteresis(normalized, null);
            }
        }

        private void EvaluateHysteresis(float normalized, IXRSelectInteractor interactor)
        {
            bool wasPressed = m_IsPressed;
            m_IsPressed = PoseMath.CalculateHysteresis(normalized, wasPressed, m_ClickThreshold, m_ReleaseThreshold);

            if (!wasPressed && m_IsPressed)
            {
                OnClicked?.Invoke();
                if (interactor != null)
                {
                    TriggerHaptic(interactor, 0.8f, 0.04f);
                }
            }
            else if (wasPressed && !m_IsPressed)
            {
                OnReleased?.Invoke();
            }
        }
    }
}
