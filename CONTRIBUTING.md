# Contributing to unity-openxr-starter

Thank you for your interest in contributing! This project is maintained to high architectural and performance standards for standalone VR/OpenXR development.

## Code of Conduct

All contributors are expected to uphold the [Code of Conduct](CODE_OF_CONDUCT.md).

## Development Philosophy

1. **Clean-Room Engineering:** All submissions must be original work or permissively licensed third-party code. Never submit code, identifiers, or assets derived from private client work.
2. **Zero Allocations in Hot Paths:** Code executing in `Update()`, `FixedUpdate()`, or interaction loops must NOT allocate managed memory on the heap. Avoid LINQ, string formatting, `GetComponent()`, and boxing.
3. **No `HingeJoint` for Rotators:** Mechanical interactables (levers, dials, doors) must calculate angles via analytical pose projection math, not PhysX joints, to eliminate VR jitter.
4. **Unity Asset Hygiene:** Every file and folder inside `Assets/` must have an accompanying, stable `.meta` file committed in the same change.
5. **Conventional Commits:** Commit messages must follow Conventional Commits: `feat:`, `fix:`, `docs:`, `chore:`, `refactor:`, `test:`, `ci:`.

## Local Verification

Before creating a pull request, run the local verification suite:

```powershell
# Fast verification: .meta completeness, GUID uniqueness, YAML headers, and clean-room guard
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/verify.ps1 fast

# Full verification: fast checks + standalone test suite + workflow linting
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/verify.ps1 full
```

## Pull Request Workflow

1. Fork the repository and create a feature branch (`feat/my-interactable`).
2. Implement your changes following modular assembly definitions.
3. Add unit tests for mathematical transformations in `tests/Foundation.StandaloneTests`.
4. Ensure `scripts/verify.ps1 full` passes with 0 failures and 0 warnings.
5. Open a pull request using the provided PR template.
