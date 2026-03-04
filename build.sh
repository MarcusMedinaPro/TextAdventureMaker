#!/bin/bash
# Build the TextAdventure project

set -e

echo "🔨 Building TextAdventure..."
dotnet build -q

echo "✅ Build successful!"
