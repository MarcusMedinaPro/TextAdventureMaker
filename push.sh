#!/bin/bash
# Push commits to remote

set -e

BRANCH=$(git rev-parse --abbrev-ref HEAD)
COMMITS=$(git rev-list --count @{u}..HEAD 2>/dev/null || echo "0")

echo "🚀 Pushing to origin/$BRANCH ($COMMITS commits)..."
git push origin "$BRANCH"

echo "✅ Push successful!"
