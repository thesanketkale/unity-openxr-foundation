#!/usr/bin/env bash
set -eo pipefail

ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
cd "$ROOT"

FORBIDDEN_PATTERNS=(
  "VueXR"
  "Trium Designs"
  "Fudge3d"
  "AKIA[0-9A-Z]{16}"
  "-----BEGIN (RSA |EC |OPENSSH )?PRIVATE KEY-----"
  "ghp_[0-9a-zA-Z]{36}"
  "gho_[0-9a-zA-Z]{36}"
)

FILES=()
if [ "$1" = "--staged" ]; then
  while IFS= read -r f; do [ -n "$f" ] && FILES+=("$f"); done < <(git diff --cached --name-only --diff-filter=ACMR)
else
  while IFS= read -r f; do [ -n "$f" ] && FILES+=("$f"); done < <(git ls-files)
fi

BAD=0
for f in "${FILES[@]}"; do
  [ -f "$f" ] || continue
  case "$f" in
    *.png|*.jpg|*.jpeg|*.ico|*.dll|*.so|*.dylib|*.bin|*.zip|*.tar|*.gz|*.aab|*.apk) continue ;;
    .git/*|.pipeline/*|tools/guard.*) continue ;;
  esac

  for p in "${FORBIDDEN_PATTERNS[@]}"; do
    if grep -E -q "$p" "$f" 2>/dev/null; then
      echo "GUARD FAILURE: $f matches forbidden pattern '$p'" >&2
      BAD=1
    fi
  done
done

if [ "$BAD" -ne 0 ]; then
  exit 1
fi

echo "Guard check passed cleanly: 0 violations."
exit 0
