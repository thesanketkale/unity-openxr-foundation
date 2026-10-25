#!/usr/bin/env bash
# scripts/verify.sh: Complete verification suite for unity-openxr-starter
set -eo pipefail

MODE="${1:-full}"
case "$MODE" in fast|full) ;; *) echo "usage: verify.sh [fast|full]" >&2; exit 64 ;; esac

ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
cd "$ROOT"

if [ -d "$HOME/.agent-team/bin" ]; then
  case ":$PATH:" in *":$HOME/.agent-team/bin:"*) ;; *) PATH="$HOME/.agent-team/bin:$PATH" ;; esac
fi

FAILED=""
SKIPPED=""
RAN=0

step() {
  local label="$1"; shift
  printf '\n> %s\n' "$label"
  RAN=$((RAN + 1))
  if "$@"; then
    printf 'ok  %s\n' "$label"
  else
    printf 'FAIL %s\n' "$label"
    FAILED="$FAILED, $label"
  fi
}

skip() {
  case "$SKIPPED," in *", $1,"*) return 0 ;; esac
  printf 'SKIPPED %s\n' "$1" >&2
  SKIPPED="$SKIPPED, $1"
}

have() {
  if command -v "$1" >/dev/null 2>&1 || [ -x "$1" ]; then return 0; fi
  skip "$1 (not installed)"
  return 1
}

# ---------------------------------------------------------------- CHECKS ----

check_guard() {
  bash tools/guard.sh
}

check_meta_files() {
  local bad=0
  # Check that every asset has a .meta file
  if [ -d "Assets" ]; then
    while IFS= read -r f; do
      case "$f" in
        *.meta|*/.*|*~|*~/*) continue ;;
      esac
      if [ ! -f "$f.meta" ]; then
        echo "Missing meta file: $f.meta" >&2
        bad=1
      fi
    done < <(find Assets -type f -o -type d)

    # Check for orphan .meta files
    while IFS= read -r m; do
      target="${m%.meta}"
      if [ ! -e "$target" ]; then
        echo "Orphan meta file with no target asset: $m" >&2
        bad=1
      fi
    done < <(find Assets -name "*.meta")
  fi
  return $bad
}

check_guid_uniqueness() {
  local bad=0
  if [ -d "Assets" ]; then
    local dups
    dups=$(find Assets -name "*.meta" -exec grep -H "^guid: " {} + | awk '{print $2}' | sort | uniq -d)
    if [ -n "$dups" ]; then
      echo "Duplicate GUIDs detected across .meta files:" >&2
      echo "$dups" >&2
      bad=1
    fi
  fi
  return $bad
}

check_asmdefs() {
  local bad=0
  if [ -d "Assets" ]; then
    while IFS= read -r asm; do
      # Validate JSON formatting
      if have python3; then
        python3 -c "import json, sys; json.load(open(sys.argv[1]))" "$asm" || bad=1
      elif have python; then
        python -c "import json, sys; json.load(open(sys.argv[1]))" "$asm" || bad=1
      fi
    done < <(find Assets -name "*.asmdef")
  fi
  return $bad
}

check_yaml_syntax() {
  local bad=0
  # Verify Unity YAML headers
  for f in $(find Assets ProjectSettings -name "*.unity" -o -name "*.prefab" -o -name "*.asset" 2>/dev/null); do
    if [ -f "$f" ]; then
      first_line=$(head -n 1 "$f")
      if [[ "$first_line" != "%YAML 1.1"* ]]; then
        echo "Invalid Unity YAML header in $f: $first_line" >&2
        bad=1
      fi
    fi
  done
  return $bad
}

check_workflows() {
  local bad=0
  if [ -d ".github/workflows" ]; then
    if have actionlint; then
      actionlint || bad=1
    fi
    if have yamllint; then
      yamllint -d "{extends: default, rules: {line-length: disable, document-start: disable, comments: disable}}" .github/workflows/ || bad=1
    fi
  fi
  return $bad
}

check_standalone_tests() {
  if [ -f "tests/Foundation.StandaloneTests/Foundation.StandaloneTests.csproj" ]; then
    dotnet test "tests/Foundation.StandaloneTests/Foundation.StandaloneTests.csproj" --nologo -v q
  else
    return 0
  fi
}

fast_checks() {
  step "clean-room & secrets guard" check_guard
  step "asset .meta file completeness & no orphans" check_meta_files
  step "GUID uniqueness across all .meta files" check_guid_uniqueness
  step "assembly definition JSON syntax" check_asmdefs
  step "Unity YAML serialization headers" check_yaml_syntax
}

full_checks() {
  step "GitHub Actions workflow linting" check_workflows
  step "standalone math and validator test suite" check_standalone_tests
}

# ---------------------------------------------------------------- RUN ----
fast_checks
if [ "$MODE" = "full" ]; then
  full_checks
fi

if [ -n "$FAILED" ]; then
  printf '\nverify %s: FAILED (%s)\n' "$MODE" "${FAILED#, }"
  exit 1
fi
if [ -n "$SKIPPED" ]; then
  printf '\nverify %s: INCOMPLETE. %d checks passed, NOT VERIFIED (skipped): %s\n' "$MODE" "$RAN" "${SKIPPED#, }"
  exit 0
fi
if [ "$RAN" -eq 0 ]; then
  printf '\nverify %s: NO CHECKS RAN.\n' "$MODE"
  if [ "$MODE" = "full" ]; then exit 3; fi
  exit 0
fi
printf '\nverify %s: OK (%d checks)\n' "$MODE" "$RAN"
