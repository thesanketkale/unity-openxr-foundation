using System;
using Foundation.Core;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace Foundation.Locomotion
{
    /// <summary>
    /// Coordinates XR player locomotion: teleportation raycasts, snap turning,
    /// continuous movement, and tunneling vignette comfort settings.
    /// </summary>
    public class XrLocomotionManager : MonoBehaviour
    {
        [Header("Teleportation")]
        [SerializeField] private TeleportationProvider m_TeleportationProvider;
        [SerializeField] private bool m_EnableTeleportation = true;

        [Header("Turning")]
        [SerializeField] private SnapTurnProvider m_SnapTurnProvider;
        [SerializeField] private ContinuousTurnProvider m_ContinuousTurnProvider;
        [SerializeField] private bool m_UseSnapTurn = true;
        [SerializeField] private float m_SnapTurnAngle = XrConstants.DefaultSnapTurnAngle;

        [Header("Comfort Vignette")]
        [SerializeField] private GameObject m_VignetteEffectObject;
        [SerializeField] private bool m_EnableComfortVignette = true;

        public bool UseSnapTurn
        {
            get => m_UseSnapTurn;
            set
            {
                m_UseSnapTurn = value;
                ApplyTurnSettings();
            }
        }

        public float SnapTurnAngle
        {
            get => m_SnapTurnAngle;
            set
            {
                m_SnapTurnAngle = value;
                ApplyTurnSettings();
            }
        }

        private void Start()
        {
            ApplySettingsFromCore();
            ApplyTurnSettings();
        }

        private void ApplySettingsFromCore()
        {
            if (XrServiceLocator.TryGet(out XrFoundationSettings settings))
            {
                m_UseSnapTurn = settings.EnableSnapTurn;
                m_SnapTurnAngle = settings.SnapTurnAngle;
                m_EnableComfortVignette = settings.EnableComfortVignette;
            }
        }

        private void ApplyTurnSettings()
        {
            if (m_SnapTurnProvider != null)
            {
                m_SnapTurnProvider.enabled = m_UseSnapTurn;
                m_SnapTurnProvider.turnAmount = m_SnapTurnAngle;
            }

            if (m_ContinuousTurnProvider != null)
            {
                m_ContinuousTurnProvider.enabled = !m_UseSnapTurn;
            }

            if (m_VignetteEffectObject != null)
            {
                m_VignetteEffectObject.SetActive(m_EnableComfortVignette);
            }
        }
    }
}
