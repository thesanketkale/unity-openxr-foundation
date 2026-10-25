using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Foundation.Core
{
    /// <summary>
    /// Coordinates runtime initialisation, tracking origin lifecycle, and XR subsystem readiness.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class XrBootstrap : MonoBehaviour
    {
        [SerializeField] private XrFoundationSettings m_Settings;
        [SerializeField] private bool m_InitializeOnAwake = true;

        private bool m_IsInitialized;

        public event Action OnXrReady;

        public bool IsInitialized => m_IsInitialized;
        public XrFoundationSettings Settings => m_Settings;

        private void Awake()
        {
            if (m_InitializeOnAwake)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            if (m_Settings == null)
            {
                m_Settings = ScriptableObject.CreateInstance<XrFoundationSettings>();
            }

            XrServiceLocator.Register(this);
            XrServiceLocator.Register(m_Settings);

            StartCoroutine(WaitForXrSubsystems());
            m_IsInitialized = true;
        }

        private IEnumerator WaitForXrSubsystems()
        {
            var displaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetSubsystems(displaySubsystems);

            float timeout = 5.0f;
            float elapsed = 0.0f;

            while (displaySubsystems.Count == 0 && elapsed < timeout)
            {
                yield return null;
                elapsed += Time.unscaledDeltaTime;
                SubsystemManager.GetSubsystems(displaySubsystems);
            }

            if (displaySubsystems.Count > 0 && displaySubsystems[0].running)
            {
                ApplyTrackingOrigin();
            }

            OnXrReady?.Invoke();
        }

        private void ApplyTrackingOrigin()
        {
            var inputSubsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(inputSubsystems);

            if (inputSubsystems.Count > 0)
            {
                TrackingOriginModeFlags targetMode = m_Settings.TrackingOriginMode == XrTrackingOrigin.Floor
                    ? TrackingOriginModeFlags.Floor
                    : TrackingOriginModeFlags.Device;

                inputSubsystems[0].TrySetTrackingOriginMode(targetMode);
            }
        }

        private void OnDestroy()
        {
            XrServiceLocator.Unregister<XrBootstrap>();
            XrServiceLocator.Unregister<XrFoundationSettings>();
        }
    }
}
