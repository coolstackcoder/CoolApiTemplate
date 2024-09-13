#!/bin/bash

# Define the destination directory in Windows
DEST_DIR="/mnt/c/Users/snoop/Downloads/ai_temp_bucket"

# Create the destination directory if it doesn't exist
mkdir -p "$DEST_DIR"

# Get the list of modified and new files
FILES=$(git ls-files --modified --others --exclude-standard)

# Check if there are any files to copy
if [ -z "$FILES" ]; then
    echo "No modified or new files to copy."
    exit 0
fi

# Copy each file to the destination directory without subdirectories
for FILE in $FILES; do
    # Get the base name of the file (removes the path)
    BASENAME=$(basename "$FILE")
    
    # Copy the file to the destination directory, overwriting if necessary
    cp "$FILE" "$DEST_DIR/$BASENAME"
done

echo "Files copied to $DEST_DIR."