#!/bin/bash
# Ladda ner .NET SDK och installera den lokalt

# Definiera versionen du vill installera
DOTNET_VERSION="7.0.100"

# Ladda ner .NET SDK i en lokal katalog
curl -sSL https://dotnet.microsoft.com/download/dotnet/thank-you/dotnet-sdk-${DOTNET_VERSION}-linux-x64-binaries | tar -xz -C /root/.dotnet

# Lägg till .NET SDK i PATH så det kan användas
export PATH=$PATH:/root/.dotnet