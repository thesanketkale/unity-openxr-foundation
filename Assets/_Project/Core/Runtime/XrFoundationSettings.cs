using UnityEngine;

namespace Foundation.Core
{
    public enum XrTrackingOrigin
    {
        Floor = 0,
        Device = 1
    }

    [CreateAssetMenu(fileName = "XrFoundationSettings", menuName = "XR Foundation/Settings", order = 100)]
    public class XrFoundationSettings : ScriptableObject
    {
        [Header("Tracking Origin")]
        [SerializeField] private XrTrackingOrigin m_TrackingOriginMode = XrTrackingOrigin.Floor;

        [Header("Locomotion & Comfort")]
        [SerializeField] private bool m_EnableSnapTurn = true;
        [SerializeField] private float m_SnapTurnAngle = XrConstants.DefaultSnapTurnAngle;
        [SerializeField] private bool m_EnableComfortVignette = true;
        [Range(0.1f, 1.0f)]
        [SerializeField] private float m_VignetteApertureSize = XrConstants.DefaultVignetteAperture;

        [Header("Feedback Defaults")]
        [Range(0f, 1f)]
        [SerializeField] private float m_DefaultHapticAmplitude = XrConstants.DefaultHapticAmplitude;
        [SerializeField] private float m_DefaultHapticDuration = XrConstants.DefaultHapticDuration;

        public XrTrackingOrigin TrackingOriginMode => m_TrackingOriginMode;
        public bool EnableSnapTurn => m_EnableSnapTurn;
        public float SnapTurnAngle => m_SnapTurnAngle;
        public bool EnableComfortVignette => m_EnableComfortVignette;
        public float VignetteApertureSize => m_VignetteApertureSize;
        public float DefaultHapticAmplitude => m_DefaultHapticAmplitude;
        public float DefaultHapticDuration => m_DefaultHapticDuration;
    }
}
