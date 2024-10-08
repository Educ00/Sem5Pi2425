#!/bin/bash
# Build the project
echo "Building the project..."
dotnet build

# Check if the build was successful
if [ $? -ne 0 ]; then
  echo "Build failed!"
  exit 1
fi

# Run the project
echo "Running the project..."
dotnet run
