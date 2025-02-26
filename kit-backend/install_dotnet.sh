#!/bin/bash
# Ladda ner .NET SDK och installera den lokalt

# Definiera versionen du vill installera
DOTNET_VERSION="7.0.100"
DOTNET_INSTALL_DIR="/root/.dotnet"

# Skapa katalogen om den inte finns
mkdir -p $DOTNET_INSTALL_DIR

# Ladda ner den rätta .NET SDK-versionen
curl -sSL https://download.visualstudio.microsoft.com/download/pr/0b321f26-55a2-4502-b118-b6c77e4f45d1/a3eb82cfe38279556a88ee745e8de99b/dotnet-sdk-${DOTNET_VERSION}-linux-x64.tar.gz -o dotnet-sdk.tar.gz

# Extrahera SDK till rätt katalog
tar -xzf dotnet-sdk.tar.gz -C $DOTNET_INSTALL_DIR

# Ta bort den nedladdade filen
rm dotnet-sdk.tar.gz

# Lägg till .NET SDK i PATH så det kan användas
export PATH=$PATH:$DOTNET_INSTALL_DIR

# Kontrollera om .NET SDK är korrekt installerad
dotnet --version