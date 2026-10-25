using System;
using Foundation.Core;
using Foundation.Core.Events;
using UnityEngine;

namespace Foundation.Input
{
    /// <summary>
    /// Central manager coordinating active input sources per hand with automatic
    /// handover between motion controllers and optical hand tracking.
    /// </summary>
    public class XrInputManager : MonoBehaviour
    {
        [Header("Mode Configuration")]
        [SerializeField] private InputDeviceMode m_DeviceMode = InputDeviceMode.Automatic;
        [SerializeField] private float m_HandoverDebounceSeconds = XrConstants.InputSwitchDebounceSeconds;

        [Header("Left Hand Sources")]
        [SerializeField] private XrControllerInputSource m_LeftControllerSource;
        [SerializeField] private XrHandTrackingInputSource m_LeftHandSource;

        [Header("Right Hand Sources")]
        [SerializeField] private XrControllerInputSource m_RightControllerSource;
        [SerializeField] private XrHandTrackingInputSource m_RightHandSource;

        private IXrInputSource m_ActiveLeftSource;
        private IXrInputSource m_ActiveRightSource;

        private float m_LeftDebounceTimer;
        private float m_RightDebounceTimer;

        public InputDeviceMode DeviceMode
        {
            get => m_DeviceMode;
            set
            {
                m_DeviceMode = value;
                EvaluateActiveSources(true);
            }
        }

        public IXrInputSource ActiveLeftSource => m_ActiveLeftSource;
        public IXrInputSource ActiveRightSource => m_ActiveRightSource;

        public event Action<XrHandedness, XrInputModality> OnModalityChanged;

        private void Awake()
        {
            XrServiceLocator.Register(this);
        }

        private void Start()
        {
            EvaluateActiveSources(true);
        }

        private void Update()
        {
            EvaluateActiveSources(false);
        }

        private void EvaluateActiveSources(bool forceImmediate)
        {
            EvaluateHand(
                XrHandedness.Left,
                m_LeftControllerSource,
                m_LeftHandSource,
                ref m_ActiveLeftSource,
                ref m_LeftDebounceTimer,
                forceImmediate);

            EvaluateHand(
                XrHandedness.Right,
                m_RightControllerSource,
                m_RightHandSource,
                ref m_ActiveRightSource,
                ref m_RightDebounceTimer,
                forceImmediate);
        }

        private void EvaluateHand(
            XrHandedness handedness,
            IXrInputSource controller,
            IXrInputSource hand,
            ref IXrInputSource activeSource,
            ref float debounceTimer,
            bool forceImmediate)
        {
            IXrInputSource candidate = DetermineCandidate(controller, hand);

            if (candidate == activeSource)
            {
                debounceTimer = 0f;
                return;
            }

            if (forceImmediate)
            {
                SwitchActiveSource(handedness, ref activeSource, candidate);
                debounceTimer = 0f;
                return;
            }

            debounceTimer += Time.unscaledDeltaTime;
            if (debounceTimer >= m_HandoverDebounceSeconds)
            {
                SwitchActiveSource(handedness, ref activeSource, candidate);
                debounceTimer = 0f;
            }
        }

        private IXrInputSource DetermineCandidate(IXrInputSource controller, IXrInputSource hand)
        {
            switch (m_DeviceMode)
            {
                case InputDeviceMode.ControllerOnly:
                    return controller;

                case InputDeviceMode.HandsOnly:
                    return hand;

                case InputDeviceMode.Automatic:
                default:
                    // If controller is currently tracked and active, prioritize controller
                    if (controller != null && controller.IsTracked)
                    {
                        return controller;
                    }
                    if (hand != null && hand.IsTracked)
                    {
                        return hand;
                    }
                    return controller ?? hand;
            }
        }

        private void SwitchActiveSource(XrHandedness handedness, ref IXrInputSource activeSource, IXrInputSource newSource)
        {
            activeSource = newSource;
            if (newSource != null)
            {
                OnModalityChanged?.Invoke(handedness, newSource.Modality);
            }
        }

        private void OnDestroy()
        {
            XrServiceLocator.Unregister<XrInputManager>();
        }
    }
}
