#!/bin/bash
# Development workflow: build + test

set -e

ACTION=${1:-"all"}

case "$ACTION" in
    all)
        echo "📦 Full development cycle..."
        ./build.sh
        ./test.sh
        echo ""
        echo "✅ All good! Ready to run or commit."
        ;;
    build)
        ./build.sh
        ;;
    test)
        ./test.sh
        ;;
    run)
        ./run.sh
        ;;
    *)
        echo "Usage: ./dev.sh [all|build|test|run]"
        echo ""
        echo "Commands:"
        echo "  all   - Build and test (default)"
        echo "  build - Build only"
        echo "  test  - Test only"
        echo "  run   - Run the demo"
        exit 1
        ;;
esac
