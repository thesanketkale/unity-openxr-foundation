# XR Performance & Optimization Guide

Maintaining high, steady frame rates is critical in standalone VR to avoid vestibular discomfort, motion sickness, and thermal throttling.

---

## 1. Frame Timing Budgets

Standalone Android headsets refresh at high fixed display rates. Missing a single frame introduces head-tracking reprojection (Application SpaceWarp / Asynchronous Spacewarp):

| Target Refresh Rate | Total Frame Budget | CPU Budget (Main + Render) | GPU Budget | Typical Headset |
|---|---|---|---|---|
| **72 Hz** | **13.88 ms** | ≤ 9.0 ms | ≤ 11.0 ms | Meta Quest 2 / Quest 3 default |
| **90 Hz** | **11.11 ms** | ≤ 7.5 ms | ≤ 9.0 ms | Meta Quest 3 / Quest Pro |
| **120 Hz** | **8.33 ms** | ≤ 5.0 ms | ≤ 6.5 ms | High-speed competitive XR |

---

## 2. Rendering Pipeline Optimizations

### Universal Render Pipeline (URP) Configuration
- **Stereo Rendering Mode:** Set to **Multiview / Single-Pass Instanced**. This draws both left and right eye views in a single draw call invocation, halving CPU draw call overhead.
- **Color Space:** **Linear** (mandatory for OpenXR).
- **Depth Texture:** Disabled unless specifically required by screen-space shaders.
- **HDR & Post-Processing:** Disabled for standalone mobile VR. Mobile GPUs suffer significant bandwidth penalties on full-screen blits.

### Fixed Foveated Rendering (FFR)
Fixed Foveated Rendering reduces pixel shading density in the peripheral vision where optical lenses blur anyway:
- Set FFR Level to **Medium** or **High** in OpenXR Project Settings.
- On Quest 3/Pro, dynamic foveated rendering can be enabled alongside eye-tracking features.

---

## 3. Draw Call & Polygon Targets for Standalone VR

| Metric | Recommended Standalone Target | Maximum Warning Limit |
|---|---|---|
| **Draw Calls (Batches)** | 50 – 120 batches | > 150 batches |
| **Triangles per Frame** | 100,000 – 350,000 | > 500,000 |
| **Active Vertices** | 100,000 – 300,000 | > 450,000 |
| **Texture Formats** | ASTC (4x4 or 6x6) | RGBA32 uncompressed |

---

## 4. Zero-Allocation Hot Path Guidelines

In mobile C# runtimes (IL2CPP + Boehm GC), garbage collection pauses freeze the render thread, causing frame drops and hitching.

### Strict Rules Enforced in `unity-openxr-starter`:
1. **No LINQ:** Avoid `Where()`, `Select()`, `ToList()`, `Any()` in `Update()`, `FixedUpdate()`, or interaction loops.
2. **No String Concatenation:** Never use `string.Format`, interpolated `$"..."`, or `+` in per-frame loops.
3. **No `GetComponent<T>()` in Loops:** Always cache component references in `Awake()` or `Start()`.
4. **Non-Allocating Physics Queries:** Use `Physics.OverlapSphereNonAlloc` and `Physics.RaycastNonAlloc` with preallocated arrays instead of allocating new array buffers every frame.
5. **PoseMath Utilities:** Mathematical transformations use pure static value types (`struct` / `Vector3` / `Quaternion`) passed by value.
