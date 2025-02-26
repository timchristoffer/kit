#!/bin/bash
# Installera .NET SDK via apt
apt-get update
apt-get install -y wget apt-transport-https software-properties-common
wget https://packages.microsoft.com/config/ubuntu/20.04/prod.list
mv prod.list /etc/apt/sources.list.d/dotnetdev.list
apt-get update
apt-get install -y dotnet-sdk-7.0