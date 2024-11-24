#!/bin/env bash

process_mermaid_document() {
  local file="$1"
  local output_file="${file%.template.*}.${file##*.}"

  # Process template documents with mermaid cli
  if mmdc -i "$file" -o "$output_file" -b transparent -t neutral; then
    git add "$output_file"
  else
    echo "Error generating document for $file" >&2
    exit 1
  fi
}

export -f process_mermaid_document

# Find and process all matching files in parallel
find . -maxdepth 1 -name "*.template.*" -print0 | xargs -0 -n 1 -I {} bash -c 'process_mermaid_document "$@"' _ {}

exit 0