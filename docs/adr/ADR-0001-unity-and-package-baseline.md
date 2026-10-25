# ADR-0001: Unity Version and Package Baseline

## Context
When bootstrapping a production foundation for standalone VR and OpenXR development, selecting the engine release and core XR package stack is the primary architectural dependency. Standalone headsets (Meta Quest 2, Quest 3, Quest 3S, Quest Pro) require strict Android SDK/NDK compatibility, Vulkan graphics pipeline stability, OpenXR specification compliance, and long-term support (LTS) lifecycle stability.

## Decision
We standardize on **Unity 6 LTS (`6000.0.28f1`)** with the following official package baseline:
- `com.unity.render-pipelines.universal`: `17.0.3` (Universal Render Pipeline)
- `com.unity.inputsystem`: `1.11.2` (New Input System)
- `com.unity.xr.openxr`: `1.13.0` (OpenXR Plugin)
- `com.unity.xr.interaction.toolkit`: `3.0.7` (XR Interaction Toolkit 3.x)
- `com.unity.xr.hands`: `1.5.0` (XR Hands package)
- `com.unity.xr.meta-openxr`: `2.1.0` (Unity OpenXR: Meta Horizon OS features)
- `com.unity.xr.core-utils`: `2.3.0` (Core XR utilities)

## Alternatives Considered
1. **Unity 2022.3 LTS:** Stable and widely deployed, but lacks modern Unity 6 GPU Resident Drawer capabilities, XR Hands 1.5 deep integrations, and optimized render graphs for standalone mobile VR.
2. **Proprietary Meta All-in-One XR SDK (Oculus Integration):** Provides vendor-specific Meta APIs but tightly couples the project to Meta hardware, breaking OpenXR portability for Pico, Vive Focus, and PC VR.
3. **SteamVR / OpenVR Legacy:** Deprecated; does not support mobile Android standalone architectures.

## Consequences
- **Positive:** Full standards compliance with Khronos OpenXR specification; portable across Meta Quest, PC VR, and other OpenXR standalone headsets; modern URP render pipeline with single-pass instanced rendering.
- **Negative:** Requires team familiarity with XR Interaction Toolkit 3.x APIs and Unity 6 LTS Editor requirements.
