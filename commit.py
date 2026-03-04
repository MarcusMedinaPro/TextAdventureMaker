#!/usr/bin/env python3
"""
Git commit helper — no shell substitution, no permission prompts.

Usage:
  python commit.py "message"               # stage all, commit
  python commit.py "message" --push        # stage all, commit, push
  python commit.py "message" file1 file2   # stage specific files, commit
  python commit.py "message" --push file1  # stage specific, commit, push
"""

import subprocess
import sys
import os

CO_AUTHOR = "Co-Authored-By: Claude <noreply@anthropic.com>"

def run(args, **kwargs):
    return subprocess.run(args, capture_output=True, text=True, **kwargs)

def die(msg):
    print(f"Error: {msg}", file=sys.stderr)
    sys.exit(1)

def main():
    args = sys.argv[1:]
    if not args:
        print(__doc__)
        sys.exit(1)

    message = args[0]
    push = "--push" in args
    files = [a for a in args[1:] if a != "--push"]

    # Stage
    if files:
        r = run(["git", "add", "--"] + files)
    else:
        r = run(["git", "add", "-A"])
    if r.returncode != 0:
        die(r.stderr.strip())

    # Check for changes
    r = run(["git", "status", "--porcelain"])
    if not r.stdout.strip():
        print("Nothing to commit.")
        return

    # Commit — message passed as a list arg, no shell involved
    full_message = f"{message}\n\n{CO_AUTHOR}"
    r = run(["git", "commit", "-m", full_message])
    if r.returncode != 0:
        die(r.stderr.strip())
    print(r.stdout.strip())

    if push:
        r = run(["git", "push"])
        if r.returncode != 0:
            die(r.stderr.strip())
        print("Pushed.")

if __name__ == "__main__":
    main()
