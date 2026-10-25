using System;
using UnityEngine;

namespace Foundation.Core.Utilities
{
    /// <summary>
    /// Non-allocating mathematical utilities for VR pose calculation,
    /// planar projection, axis constraints, and hysteresis thresholds.
    /// </summary>
    public static class PoseMath
    {
        private const float Epsilon = 1e-6f;

        /// <summary>
        /// Projects a 3D point onto a plane defined by an origin and normal.
        /// </summary>
        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            Vector3 diff = point - planeOrigin;
            float distance = Vector3.Dot(diff, planeNormal);
            return point - (planeNormal * distance);
        }

        /// <summary>
        /// Projects a point onto a 1D linear axis line segment and computes clamped normalized progress.
        /// </summary>
        public static Vector3 ProjectPointOnAxis(
            Vector3 point,
            Vector3 axisOrigin,
            Vector3 axisDirection,
            float minDistance,
            float maxDistance,
            out float normalized)
        {
            Vector3 normalizedAxis = axisDirection.normalized;
            Vector3 pointVector = point - axisOrigin;
            float projectedDistance = Vector3.Dot(pointVector, normalizedAxis);

            float clampedDistance = Mathf.Clamp(projectedDistance, minDistance, maxDistance);
            float range = maxDistance - minDistance;

            normalized = range > Epsilon ? (clampedDistance - minDistance) / range : 0f;
            return axisOrigin + (normalizedAxis * clampedDistance);
        }

        /// <summary>
        /// Calculates the signed angle in degrees around a given rotation axis between a reference vector and a target vector.
        /// </summary>
        public static float ComputeSignedAngleAroundAxis(Vector3 targetVector, Vector3 referenceVector, Vector3 rotationAxis)
        {
            Vector3 projTarget = ProjectPointOnPlane(targetVector, Vector3.zero, rotationAxis);
            Vector3 projRef = ProjectPointOnPlane(referenceVector, Vector3.zero, rotationAxis);

            if (projTarget.sqrMagnitude < Epsilon || projRef.sqrMagnitude < Epsilon)
            {
                return 0f;
            }

            return Vector3.SignedAngle(projRef, projTarget, rotationAxis);
        }

        /// <summary>
        /// Normalizes a continuous angle within a specified [min, max] range into a 0.0 to 1.0 scalar.
        /// </summary>
        public static float NormalizeAngle(float angleDegrees, float minDegrees, float maxDegrees)
        {
            float clamped = Mathf.Clamp(angleDegrees, minDegrees, maxDegrees);
            float range = maxDegrees - minDegrees;
            return range > Epsilon ? (clamped - minDegrees) / range : 0f;
        }

        /// <summary>
        /// Applies hysteresis threshold logic to prevent chatter on binary activations (e.g. push buttons or grips).
        /// </summary>
        public static bool CalculateHysteresis(float currentValue, bool wasActive, float activationThreshold, float deactivationThreshold)
        {
            if (wasActive)
            {
                return currentValue >= deactivationThreshold;
            }
            return currentValue >= activationThreshold;
        }

        /// <summary>
        /// Smoothly clamps and wraps an angle around 360 degrees.
        /// </summary>
        public static float WrapAngle360(float angle)
        {
            angle %= 360f;
            if (angle < 0f)
            {
                angle += 360f;
            }
            return angle;
        }
    }
}
