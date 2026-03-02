#!/usr/bin/env python3
"""
Build and test automation script.
Usage: python build_and_test.py
"""

import subprocess
import sys

def run_command(cmd, description):
    """Run a shell command and report results."""
    print(f"\n{'='*60}")
    print(f"{description}")
    print(f"{'='*60}")

    result = subprocess.run(cmd, shell=True, text=True)
    return result.returncode == 0

def main():
    # Build
    if not run_command(
        "dotnet build src/MarcusMedina.TextAdventure/",
        "Building project..."
    ):
        print("❌ Build failed")
        sys.exit(1)

    print("✓ Build successful")

    # Test
    if not run_command(
        "dotnet test tests/MarcusMedina.TextAdventure.Tests/ --verbosity=minimal",
        "Running tests..."
    ):
        print("❌ Tests failed")
        sys.exit(1)

    print("✓ All tests passed")

if __name__ == "__main__":
    main()
