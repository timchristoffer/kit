#!/bin/bash
curl -sSL https://download.visualstudio.microsoft.com/download/pr/7a2e3f36-47e9-4638-bcb3-d598a0758a3e/9fc40566d72b5ac4e1d38f9704f17398/dotnet-sdk-7.0.100-linux-x64.tar.gz | tar -xvzf - -C /root/.dotnet
export PATH=$PATH:/root/.dotnet