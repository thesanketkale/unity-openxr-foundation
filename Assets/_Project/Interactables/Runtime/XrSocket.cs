using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Foundation.Interactables
{
    /// <summary>
    /// Snapping socket with a ghost holographic preview material when compatible objects hover.
    /// </summary>
    public class XrSocket : XRSocketInteractor
    {
        [Header("Ghost Preview")]
        [SerializeField] private GameObject m_GhostPreviewObject;
        [SerializeField] private Material m_GhostPreviewMaterial;
        [SerializeField] private string m_RequiredTag = "Socketable";

        [Header("Snap Behavior")]
        [SerializeField] private bool m_SmoothSnap = true;
        [SerializeField] private float m_SnapSpeed = 15f;

        private Renderer[] m_GhostRenderers;

        protected override void Awake()
        {
            base.Awake();
            if (m_GhostPreviewObject != null)
            {
                m_GhostRenderers = m_GhostPreviewObject.GetComponentsInChildren<Renderer>(true);
                SetGhostVisible(false);
            }
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            if (!base.CanHover(interactable)) return false;
            return MatchesSocketCriteria(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (!base.CanSelect(interactable)) return false;
            return MatchesSocketCriteria(interactable);
        }

        private bool MatchesSocketCriteria(IXRInteractable interactable)
        {
            if (string.IsNullOrEmpty(m_RequiredTag)) return true;
            return interactable.transform.CompareTag(m_RequiredTag);
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (!hasSelection)
            {
                SetGhostVisible(true);
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (interactorsHovering.Count == 0)
            {
                SetGhostVisible(false);
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            SetGhostVisible(false);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
        }

        private void SetGhostVisible(bool visible)
        {
            if (m_GhostPreviewObject != null)
            {
                m_GhostPreviewObject.SetActive(visible);
            }
        }
    }
}
