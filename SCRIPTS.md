# TextAdventure Helper Scripts

Quick scripts for development workflows. No permission prompts needed!

## Available Scripts

### `./dev.sh` - Development Workflow (Recommended)
```bash
./dev.sh              # Build + test
./dev.sh all          # Same as above
./dev.sh build        # Build only
./dev.sh test         # Test only
./dev.sh run          # Run the demo
```

### `./build.sh` - Build Only
```bash
./build.sh
```

### `./test.sh` - Run Tests
```bash
./test.sh
```

### `./run.sh` - Run the Demo
```bash
./run.sh
```

### `./commit.sh` - Create a Commit
```bash
./commit.sh "feat: add new feature"
./commit.sh "fix: resolve NPC reaction issue"
./commit.sh "docs: update documentation"
```

Automatically adds co-author attribution. Shows changes before prompting for confirmation.

### `./push.sh` - Push to Remote
```bash
./push.sh
```

## Typical Workflow

```bash
# 1. Make changes to files
# 2. Build and test
./dev.sh

# 3. Commit
./commit.sh "feat: implement new feature"

# 4. Push
./push.sh

# 5. Or run the demo to test interactively
./dev.sh run
```

## Quick One-Liners

```bash
# Build, test, and run in sequence
./dev.sh && ./dev.sh run

# Build and test everything
./dev.sh all && git status

# Just run tests
./test.sh

# Commit and push
./commit.sh "your message" && ./push.sh
```
