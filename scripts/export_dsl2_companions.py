#!/usr/bin/env python3
from __future__ import annotations

import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SOURCE_DIR = ROOT / "Demo-Adventures"
OUTPUT_DIR = SOURCE_DIR / "DSL2"

DSL_BLOCK_PATTERN = re.compile(r'string\s+dsl\s*=\s*"""\n(.*?)\n"""\s*;', re.DOTALL)


def main() -> int:
    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)

    extracted: list[str] = []
    missing: list[str] = []

    for markdown_file in sorted(SOURCE_DIR.glob("DA_*.md")):
        text = markdown_file.read_text(encoding="utf-8")
        match = DSL_BLOCK_PATTERN.search(text)
        if match is None:
            missing.append(markdown_file.name)
            continue

        dsl_text = match.group(1).rstrip() + "\n"
        output_file = OUTPUT_DIR / markdown_file.with_suffix(".dsl").name
        output_file.write_text(dsl_text, encoding="utf-8")
        extracted.append(output_file.name)

    write_readme(extracted, missing)

    print(f"Total demos: {len(extracted) + len(missing)}")
    print(f"DSL2 companions: {len(extracted)}")
    print(f"Pending DSL2 ports: {len(missing)}")
    return 0


def write_readme(extracted: list[str], missing: list[str]) -> None:
    lines: list[str] = [
        "# Demo Adventures DSL2 Companions",
        "",
        "This folder contains pure DSL2 source companions extracted from demo adventures when available.",
        "",
        "Current status:",
        f"- Total demo adventures: {len(extracted) + len(missing)}",
        f"- DSL2 companion available: {len(extracted)}",
        f"- Pending DSL2 port: {len(missing)}",
        "",
        "Available now:",
    ]

    lines.extend([f"- `{name}`" for name in extracted] or ["- _None yet_"])
    lines.append("")
    lines.append("Pending port (currently C#-only demos):")
    lines.extend([f"- `{name}`" for name in missing] or ["- _None_"])
    lines.append("")

    (OUTPUT_DIR / "README.md").write_text("\n".join(lines), encoding="utf-8")


if __name__ == "__main__":
    raise SystemExit(main())
