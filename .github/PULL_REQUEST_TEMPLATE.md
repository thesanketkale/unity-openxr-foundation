## Description
<!-- Provide a brief summary of the changes introduced by this pull request. -->

## Type of Change
- [ ] `feat`: New feature or interactable primitive
- [ ] `fix`: Bug fix
- [ ] `refactor`: Code refactoring without behavior change
- [ ] `docs`: Documentation update or new ADR
- [ ] `ci`: CI/CD workflow adjustment

## Architectural & Quality Checklist
- [ ] Conforms to Conventional Commits format (`type(scope): description`).
- [ ] Zero allocations in hot paths (`Update`, `FixedUpdate`).
- [ ] Every new asset has an accompanying `.meta` file committed.
- [ ] No `HingeJoint` components used for mechanical rotators (uses analytical projection math).
- [ ] Standalone test suite passes (`dotnet test tests/Foundation.StandaloneTests/...`).
- [ ] Local verification check passes (`powershell -NoProfile -ExecutionPolicy Bypass -File scripts/verify.ps1 full`).
- [ ] Clean-room compliance: zero proprietary code, credentials, or client names.
