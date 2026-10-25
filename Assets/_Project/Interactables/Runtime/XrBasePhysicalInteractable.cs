using System;
using Foundation.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Abstract base class for all physical mechanical interactables.
    /// Exposes normalized 0..1 values, value change events, and haptic/audio cues.
    /// </summary>
    public abstract class XrBasePhysicalInteractable : XRBaseInteractable
    {
        [Header("Normalized Value")]
        [Range(0f, 1f)]
        [SerializeField] private float m_NormalizedValue;
        [SerializeField] private float m_ValueChangeEpsilon = 0.001f;

        [Header("Audio Feedback")]
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_InteractionStartClip;
        [SerializeField] private AudioClip m_InteractionEndClip;
        [SerializeField] private AudioClip m_ValueThresholdClip;

        [Header("Haptic Feedback")]
        [Range(0f, 1f)]
        [SerializeField] private float m_HapticIntensity = XrConstants.DefaultHapticAmplitude;
        [SerializeField] private float m_HapticDuration = XrConstants.DefaultHapticDuration;

        [Header("Events")]
        [SerializeField] private UnityEvent<float> m_OnValueChanged = new UnityEvent<float>();

        public float Normalized => m_NormalizedValue;
        public UnityEvent<float> OnValueChanged => m_OnValueChanged;
        public event Action<float> ValueChanged;

        public bool HasActiveInteractor => isSelected || isHovered;

        protected virtual void SetNormalizedValue(float value, bool notify = true)
        {
            float clamped = Mathf.Clamp01(value);
            if (Mathf.Abs(clamped - m_NormalizedValue) > m_ValueChangeEpsilon)
            {
                m_NormalizedValue = clamped;
                OnNormalizedValueChanged(m_NormalizedValue);

                if (notify)
                {
                    m_OnValueChanged?.Invoke(m_NormalizedValue);
                    ValueChanged?.Invoke(m_NormalizedValue);
                }
            }
        }

        protected virtual void OnNormalizedValueChanged(float newValue)
        {
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            PlayAudio(m_InteractionStartClip);
            TriggerHaptic(args.interactorObject, m_HapticIntensity, m_HapticDuration);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            PlayAudio(m_InteractionEndClip);
            TriggerHaptic(args.interactorObject, m_HapticIntensity * 0.5f, m_HapticDuration);
        }

        public void PlayAudio(AudioClip clip)
        {
            if (clip != null && m_AudioSource != null)
            {
                m_AudioSource.PlayOneShot(clip);
            }
        }

        public void TriggerHaptic(IXRInteractor interactor, float intensity, float duration)
        {
            if (interactor is XRBaseInputInteractor inputInteractor)
            {
                inputInteractor.SendHapticImpulse(intensity, duration);
            }
        }
    }
}
