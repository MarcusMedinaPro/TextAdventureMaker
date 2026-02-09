#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARTIFACTS_DIR="${ROOT_DIR}/artifacts"

mkdir -p "${ARTIFACTS_DIR}"

dotnet pack "${ROOT_DIR}/src/MarcusMedina.TextAdventure/MarcusMedina.TextAdventure.csproj" -c Release -o "${ARTIFACTS_DIR}"
dotnet pack "${ROOT_DIR}/src/MarcusMedina.TextAdventure.AI/MarcusMedina.TextAdventure.AI.csproj" -c Release -o "${ARTIFACTS_DIR}"

if [[ -n "${NUGET_API_KEY:-}" ]]; then
  dotnet nuget push "${ARTIFACTS_DIR}/*.nupkg" --api-key "${NUGET_API_KEY}" --source "https://api.nuget.org/v3/index.json" --skip-duplicate
else
  echo "NUGET_API_KEY not set; packages packed to ${ARTIFACTS_DIR}."
fi
