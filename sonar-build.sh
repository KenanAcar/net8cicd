#!/bin/bash

SONAR_PROJECT_KEY="dotnet8cicd"
SONAR_HOST_URL="http://localhost:9000"
SONAR_TOKEN="sqp_0c0fb5b08a9f69e3c21653b13583b91a896d1825" 

if [ -z "$SONAR_TOKEN" ]; then
  echo "❌ SONAR_TOKEN environment variable is not set."
  exit 1
fi

dotnet sonarscanner begin \
  /k:"$SONAR_PROJECT_KEY" \
  /d:sonar.host.url="$SONAR_HOST_URL" \
  /d:sonar.login="$SONAR_TOKEN"

dotnet build

dotnet sonarscanner end \
  /d:sonar.login="$SONAR_TOKEN"
