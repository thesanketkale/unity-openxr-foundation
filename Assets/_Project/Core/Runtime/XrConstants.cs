namespace Foundation.Core
{
    /// <summary>
    /// Global constants for XR layer masks, interaction profiles, and comfort defaults.
    /// </summary>
    public static class XrConstants
    {
        // Physics & Raycast Layers
        public const string DefaultLayerName = "Default";
        public const string InteractableLayerName = "Interactable";
        public const string TeleportLayerName = "Teleport";
        public const string UiLayerName = "UI";

        public const int InteractableLayerIndex = 8;
        public const int TeleportLayerIndex = 9;

        // OpenXR Interaction Profiles
        public const string OculusTouchProfile = "/interaction_profiles/oculus/touch_controller";
        public const string MetaQuestTouchProProfile = "/interaction_profiles/meta/touch_pro_controller";
        public const string MetaQuestTouchPlusProfile = "/interaction_profiles/meta/touch_plus_controller";
        public const string KhronosHandInteractionProfile = "/interaction_profiles/ext/hand_interaction_ext";
        public const string MicrosoftHandInteractionProfile = "/interaction_profiles/microsoft/hand_interaction";

        // Comfort & Locomotion Defaults
        public const float DefaultSnapTurnAngle = 45.0f;
        public const float DefaultSmoothTurnSpeed = 90.0f;
        public const float DefaultVignetteAperture = 0.6f;
        public const float DefaultVignetteFeathering = 0.2f;

        // Interaction Thresholds
        public const float DefaultHapticDuration = 0.05f;
        public const float DefaultHapticAmplitude = 0.5f;
        public const float ButtonPokeHysteresisUpper = 0.80f;
        public const float ButtonPokeHysteresisLower = 0.40f;
        public const float InputSwitchDebounceSeconds = 0.25f;
    }
}
