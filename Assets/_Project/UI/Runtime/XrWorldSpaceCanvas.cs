using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Foundation.UI
{
    /// <summary>
    /// Configures a world-space UI Canvas for VR ray and poke interaction.
    /// Ensures proper BoxCollider sizing, TrackedDeviceGraphicRaycaster, and event camera binding.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(TrackedDeviceGraphicRaycaster))]
    public class XrWorldSpaceCanvas : MonoBehaviour
    {
        [SerializeField] private BoxCollider m_PokeCollider;
        [SerializeField] private float m_PokeColliderDepth = 0.02f; // 2 cm

        private Canvas m_Canvas;
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            m_RectTransform = GetComponent<RectTransform>();

            ConfigureCanvas();
            ConfigureCollider();
        }

        private void ConfigureCanvas()
        {
            m_Canvas.renderMode = RenderMode.WorldSpace;
            if (m_Canvas.worldCamera == null && Camera.main != null)
            {
                m_Canvas.worldCamera = Camera.main;
            }
        }

        private void ConfigureCollider()
        {
            if (m_PokeCollider == null)
            {
                m_PokeCollider = GetComponent<BoxCollider>();
                if (m_PokeCollider == null)
                {
                    m_PokeCollider = gameObject.AddComponent<BoxCollider>();
                }
            }

            Vector2 size = m_RectTransform.rect.size;
            m_PokeCollider.size = new Vector3(size.x, size.y, m_PokeColliderDepth);
            m_PokeCollider.center = new Vector3(0f, 0f, m_PokeColliderDepth * 0.5f);
        }
    }
}
