#!/bin/bash
# Run tests for TextAdventure project

set -e

echo "🧪 Running tests..."
dotnet test -q

echo "✅ All tests passed!"
