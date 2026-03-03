#!/usr/bin/env python3
"""
Build, test and run helper — no shell substitution, no permission prompts.

Usage:
  python build_and_test.py              # build + test (whole solution)
  python build_and_test.py --run        # build + run sandbox
  python build_and_test.py --build      # build only
  python build_and_test.py --test       # test only
  python build_and_test.py --sandbox    # run sandbox only (no build)
"""

import subprocess
import sys

SOLUTION   = "/mnt/c/git/TextAdventureMaker"
SANDBOX    = f"{SOLUTION}/sandbox/TextAdventure.Sandbox"
CORE       = f"{SOLUTION}/src/MarcusMedina.TextAdventure"
AI         = f"{SOLUTION}/src/MarcusMedina.TextAdventure.AI"
TESTS      = f"{SOLUTION}/tests/MarcusMedina.TextAdventure.Tests"

def run(args, cwd=None):
    print(f"\n> {' '.join(args)}")
    result = subprocess.run(args, cwd=cwd)
    return result.returncode == 0

def build():
    return run(["dotnet", "build", SOLUTION, "-q"])

def test():
    return run(["dotnet", "test", TESTS, "--verbosity=minimal"])

def sandbox():
    return run(["dotnet", "run", "--project", SANDBOX])

def main():
    args = sys.argv[1:]

    if "--sandbox" in args:
        if not sandbox():
            sys.exit(1)
        return

    if "--build" in args:
        if not build():
            print("Build failed.")
            sys.exit(1)
        print("Build OK.")
        return

    if "--test" in args:
        if not test():
            print("Tests failed.")
            sys.exit(1)
        print("Tests OK.")
        return

    if "--run" in args:
        if not build():
            print("Build failed.")
            sys.exit(1)
        sandbox()
        return

    # Default: build + test
    if not build():
        print("Build failed.")
        sys.exit(1)
    if not test():
        print("Tests failed.")
        sys.exit(1)
    print("\nAll good.")

if __name__ == "__main__":
    main()
