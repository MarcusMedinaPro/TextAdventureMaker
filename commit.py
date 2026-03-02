#!/usr/bin/env python3
"""
Git commit automation script.
Usage: python commit.py "commit message"
"""

import subprocess
import sys
import os

def run_command(cmd):
    """Run a shell command without prompting."""
    result = subprocess.run(cmd, shell=True, capture_output=True, text=True)
    return result.returncode, result.stdout, result.stderr

def commit(message):
    """Commit changes with the given message."""
    if not message:
        print("Error: No commit message provided")
        sys.exit(1)

    # Stage all changes
    print("Staging changes...")
    returncode, stdout, stderr = run_command("git add -A")
    if returncode != 0:
        print(f"Error staging changes: {stderr}")
        sys.exit(1)

    # Check if there are changes to commit
    returncode, stdout, stderr = run_command("git status --porcelain")
    if not stdout.strip():
        print("No changes to commit")
        return

    # Commit with the provided message
    print(f"Committing: {message}")
    commit_cmd = f'git commit -m "{message}\n\nCo-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>"'
    returncode, stdout, stderr = run_command(commit_cmd)

    if returncode != 0:
        print(f"Error committing: {stderr}")
        sys.exit(1)

    print(stdout)
    print("✓ Commit successful")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python commit.py \"commit message\"")
        sys.exit(1)

    message = sys.argv[1]
    commit(message)
