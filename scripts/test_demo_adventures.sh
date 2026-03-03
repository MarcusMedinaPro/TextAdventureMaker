#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MODE="smoke"
GLOB_PATTERN="Demo-Adventures/DA_*.md"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --deep)
      MODE="deep"
      shift
      ;;
    --smoke)
      MODE="smoke"
      shift
      ;;
    --pattern)
      GLOB_PATTERN="${2:-}"
      shift 2
      ;;
    *)
      echo "Unknown option: $1" >&2
      echo "Usage: scripts/test_demo_adventures.sh [--smoke|--deep] [--pattern <glob>]" >&2
      exit 2
      ;;
  esac
done

cd "$ROOT_DIR"
SUMMARY_FILE="$(mktemp)"
trap 'rm -f "$SUMMARY_FILE"' EXIT

printf "file\tcopy\tbuild\trun\tissues\tnote\n" > "$SUMMARY_FILE"

for file in $GLOB_PATTERN; do
  [[ -f "$file" ]] || continue

  if ! python3 scripts/example_to_sandbox.py "$file" >/tmp/da_copy.log 2>&1; then
    note="$(tail -n 1 /tmp/da_copy.log | tr '\t' ' ')"
    printf "%s\tfail\t-\t-\tyes\t%s\n" "$file" "$note" >> "$SUMMARY_FILE"
    continue
  fi

  if ! dotnet build sandbox/TextAdventure.Sandbox/ --nologo >/tmp/da_build.log 2>&1; then
    # WSL + Windows file locking can leave sandbox DLLs locked; clean and retry once.
    if rg -q "MSB3021|Access to the path .* is denied" /tmp/da_build.log; then
      dotnet clean sandbox/TextAdventure.Sandbox/ --nologo >/tmp/da_clean.log 2>&1 || true
      if ! dotnet build sandbox/TextAdventure.Sandbox/ --nologo >/tmp/da_build.log 2>&1; then
        note="$(rg -n 'error [A-Z0-9]+:' /tmp/da_build.log | head -n 2 | tr '\n' ' ' | tr '\t' ' ' || true)"
        [[ -n "$note" ]] || note="$(tail -n 4 /tmp/da_build.log | tr '\n' ' ' | tr '\t' ' ')"
        printf "%s\tok\tfail\t-\tyes\t%s\n" "$file" "$note" >> "$SUMMARY_FILE"
        continue
      fi
    else
      note="$(rg -n 'error [A-Z0-9]+:' /tmp/da_build.log | head -n 2 | tr '\n' ' ' | tr '\t' ' ' || true)"
      [[ -n "$note" ]] || note="$(tail -n 4 /tmp/da_build.log | tr '\n' ' ' | tr '\t' ' ')"
      printf "%s\tok\tfail\t-\tyes\t%s\n" "$file" "$note" >> "$SUMMARY_FILE"
      continue
    fi
  fi

  if [[ "$MODE" == "deep" ]]; then
    INPUT=$'look\ninventory\nread case note\ngo north\ntalk watcher\ngo north\ntake proof token\ngo south\ngo south\ngo east\nlook\ngo west\nquit'
    ISSUE_PATTERN="Unknown command|can't go that way|No one to talk to|No such npc here|Nothing to read|No such item here|No such item in inventory"
  else
    INPUT=$'look\nquit'
    ISSUE_PATTERN="Unknown command"
  fi

  if ! dotnet run --project sandbox/TextAdventure.Sandbox/ --no-build >/tmp/da_run.log 2>&1 <<<"$INPUT"; then
    note="$(tail -n 2 /tmp/da_run.log | tr '\n' ' ' | tr '\t' ' ')"
    printf "%s\tok\tok\tfail\tyes\t%s\n" "$file" "$note" >> "$SUMMARY_FILE"
    continue
  fi

  issues="no"
  note=""
  if rg -n "$ISSUE_PATTERN" /tmp/da_run.log >/tmp/da_issues.log 2>&1; then
    issues="yes"
    note="$(head -n 3 /tmp/da_issues.log | tr '\n' ' ')"
  fi

  printf "%s\tok\tok\tok\t%s\t%s\n" "$file" "$issues" "$note" >> "$SUMMARY_FILE"
done

column -ts $'\t' "$SUMMARY_FILE"
