using System;
using Foundation.Core.Events;
using UnityEngine;

namespace Foundation.Input
{
    /// <summary>
    /// Non-allocating value type representing current hand joint poses and normalized gesture strengths.
    /// </summary>
    public struct XrHandPoseData
    {
        public XrHandedness Handedness;
        public XrTrackingConfidence Confidence;
        public Pose WristPose;
        public Pose IndexTipPose;
        public Pose ThumbTipPose;
        public float PinchStrength; // 0.0f (open) to 1.0f (pinched)
        public float GripStrength;  // 0.0f (open) to 1.0f (fist)
        public bool IsPinching;
        public bool IsGripping;
        public bool IsPointerActive;
    }
}
