using System;
using UnityEngine;

namespace Foundation.Core.Events
{
    public enum XrTrackingConfidence
    {
        Untracked = 0,
        Low = 1,
        High = 2
    }

    public enum XrHandedness
    {
        Left = 0,
        Right = 1
    }

    public enum XrInputModality
    {
        Controller = 0,
        HandTracking = 1
    }

    public readonly struct TrackingStateChangedEvent
    {
        public readonly XrHandedness Handedness;
        public readonly XrTrackingConfidence Confidence;
        public readonly XrInputModality Modality;

        public TrackingStateChangedEvent(XrHandedness handedness, XrTrackingConfidence confidence, XrInputModality modality)
        {
            Handedness = handedness;
            Confidence = confidence;
            Modality = modality;
        }
    }

    public readonly struct InputModalityChangedEvent
    {
        public readonly XrHandedness Handedness;
        public readonly XrInputModality PreviousModality;
        public readonly XrInputModality CurrentModality;

        public InputModalityChangedEvent(XrHandedness handedness, XrInputModality previousModality, XrInputModality currentModality)
        {
            Handedness = handedness;
            PreviousModality = previousModality;
            CurrentModality = currentModality;
        }
    }

    public readonly struct InteractableValueChangedEvent
    {
        public readonly string InteractableId;
        public readonly float NormalizedValue;
        public readonly bool IsInteracted;

        public InteractableValueChangedEvent(string interactableId, float normalizedValue, bool isInteracted)
        {
            InteractableId = interactableId;
            NormalizedValue = normalizedValue;
            IsInteracted = isInteracted;
        }
    }
}
