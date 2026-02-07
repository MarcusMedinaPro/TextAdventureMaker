#!/usr/bin/env python3
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
EXAMPLES_DIR = ROOT / "docs" / "examples"
SANDBOX_PROGRAM = ROOT / "sandbox" / "TextAdventure.Sandbox" / "Program.cs"


def find_example(arg: str) -> Path:
    candidate = Path(arg)
    if candidate.suffix == ".md" and candidate.exists():
        return candidate

    if (EXAMPLES_DIR / f"{arg}.md").exists():
        return EXAMPLES_DIR / f"{arg}.md"

    if arg.isdigit():
        num = int(arg)
        patterns = [f"{num:02d}_", f"{num}_"]
        matches = [p for p in EXAMPLES_DIR.glob("*.md") if any(p.name.startswith(prefix) for prefix in patterns)]
        if len(matches) == 1:
            return matches[0]
        if len(matches) > 1:
            raise SystemExit(f"Multiple examples match slice {arg}: {', '.join(p.name for p in matches)}")

    lower = arg.lower().replace("-", "_")
    matches = [p for p in EXAMPLES_DIR.glob("*.md") if lower in p.stem.lower()]
    if len(matches) == 1:
        return matches[0]
    if len(matches) > 1:
        raise SystemExit(f"Multiple examples match '{arg}': {', '.join(p.name for p in matches)}")

    raise SystemExit(f"No example found for '{arg}'.")


def replace_csharp_block(text: str, code: str) -> str:
    match = re.search(r"```csharp\n(.*?)\n```", text, re.DOTALL)
    if not match:
        raise SystemExit("No csharp code block found.")

    new_block = "```csharp\n" + code.rstrip() + "\n```"
    return text[:match.start()] + new_block + text[match.end():]


def main() -> int:
    if len(sys.argv) != 2:
        print("Usage: scripts/sandbox_to_example.py <example name or slice number>")
        return 2

    example = find_example(sys.argv[1])
    code = SANDBOX_PROGRAM.read_text(encoding="utf-8")
    content = example.read_text(encoding="utf-8")
    updated = replace_csharp_block(content, code)

    example.write_text(updated, encoding="utf-8")
    print(f"Updated {example.relative_to(ROOT)} from sandbox program")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
