#!/usr/bin/env bash
set -euo pipefail
BASE_DIR=$(realpath "$(dirname "$0")")
PKG_DIR=$BASE_DIR
OUT_DIR=$BASE_DIR/out
VERSION=$(jq -r .version "$PKG_DIR/package.json")
OUT_FILE=$OUT_DIR/ms.sora.badge-kit-$VERSION.zip
mkdir -p "$OUT_DIR"
cd "$PKG_DIR"
rm -f "$OUT_FILE"
7z a -r -x'!build_package.sh' -x'!out' -- "$OUT_FILE" ./*
