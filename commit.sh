#!/bin/bash
# Create a commit with a message

if [ -z "$1" ]; then
    echo "Usage: ./commit.sh <message>"
    echo "Example: ./commit.sh 'feat: add new feature'"
    exit 1
fi

set -e

MESSAGE="$1"

echo "📝 Staging changes..."
git add -A

echo "📊 Changes to commit:"
git diff --cached --stat

echo ""
read -p "Confirm commit? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Commit cancelled"
    git reset
    exit 1
fi

echo "💾 Committing..."
git commit -m "$MESSAGE

Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>"

echo "✅ Commit successful!"
echo ""
git log -1 --oneline
