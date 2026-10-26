# AGENTS.md: unity-openxr-foundation

Shared rules for every AI agent on this repo, in Claude Code (through CLAUDE.md) and Google Antigravity (which reads this file directly). Keep it under ~150 lines. It is read at the start of every run. Write `TBD` rather than guessing.

## Project
- What it is: Unity OpenXR project foundation for standalone headsets: modular assembly layout, hand and controller input, physical interactables, project validation and scripted builds.
- Users: Unity XR developers building standalone headset applications (Meta Quest and OpenXR Android devices).
- Related repos and contracts: unity-build-pipeline (CI/CD workflows), unity-ui-context (UI inspection).

## Stack packs
<!-- One per line. Agents load {skills dir}/stack-<name>/SKILL.md for each. -->
- unity-xr
- github-actions

## Commands
- Install: Open project in Unity 6 LTS Editor (UNITY_PATH)
- Run locally: None (owner opens project in Unity Editor)
- Verify fast: `bash scripts/verify.sh fast` (Windows PowerShell: `./scripts/verify.ps1 fast`)
- Verify full: `bash scripts/verify.sh full`
- Disposable database: none

## Git
- Base branch: main
- Branches: `feat/<slug>`, `fix/<slug>`, `chore/<slug>`
- PR: manual

## Conventions
- This is a public MIT repository written clean-room: no code, assets, names or data from client or employer projects. Sample data is synthetic.
- The standards and this repository's requirements are in `.pipeline/SPEC.md` (local, never committed). Requirement IDs appear in AC lines as `AC-n [ID]` and in test names.
- Docs describe only what exists and was run. What an agent cannot run (push, live cloud, signing, a Unity Editor, GitHub settings) is a `[USER]` item in `docs/release-checklist.md` and is never described as tested.
- GitHub Actions: top-level `permissions: {}` plus minimal per-job permissions; `timeout-minutes` and `concurrency` on every workflow; actions pinned to a full SHA with the version in a comment; explicit `shell:`; LF line endings; no `${{ github.event.* }}` inside `run:` (use `env:`); OIDC for cloud login. Check with `actionlint`.
- Commits are small and follow `<type>(<scope>): <goal> [step N]`. Dates are real and history is never rewritten.
- Dependencies need MIT-compatible licences. Record stack choices and open-decision outcomes in `docs/adr/`.
- Project settings are part of this spec. Player Settings, XR Plug-in Management and `Packages/manifest.json` change only as the plan says and the gate approved.
- Code lives under `Assets/_Project/`, one assembly definition per module (`Foundation.Core`, `.Input`, `.Interactables`, `.UI`, `.Locomotion`, `.Editor`) with test assemblies beside them. Assembly references have no cycles.
- Every file and folder under `Assets/` and `Packages/` has a `.meta` with a unique GUID. Agents create them with `scripts/gen-meta.mjs`, never copy a GUID, and never edit a `.meta` by hand.
- Scenes and prefabs are created by an editor script run through `UNITY_PATH`, not typed as YAML by hand. Without `UNITY_PATH` they stay a `[USER]` item.
- Nothing allocates in `Update`. Levers and dials never use `HingeJoint`.

## Design
- Direction: Visuals come from primitives and original materials. World-space UI is checked on headset by the owner (`--no-design`). No imported Unity sample assets and no third-party assets.

## Safety: agents never do these. They prepare the command and hand it over.
- Deploy, publish, release, upload, sign or notarize builds
- `terraform` / `terragrunt` apply, destroy, import or any state command
- Any write to a database other than the disposable one above
- Change real secrets, `.env` values, CI/CD or production configuration
- Push to the base branch, force-push, rewrite published history
- Never push, tag, create releases, change GitHub settings or run `git filter-repo`; write the command into `docs/release-checklist.md` instead
- Never print secret values: show only file, line, rule and the last four characters
- Agents never open the Unity Editor window and never run a signed build
- Scenes and prefabs are created by editor scripts run through `UNITY_PATH`, never typed as YAML by hand

## Known baseline failures
<!-- Pre-existing failures moved out of `fast`, with a note on why. Remove as they get fixed. -->

## Lessons
<!-- One line each, added only with the user's approval after a review found a repeatable mistake.
     Format: - [area] Do X, not Y, because Z. -->
