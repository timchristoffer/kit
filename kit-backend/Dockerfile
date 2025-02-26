# Använd den officiella .NET SDK-bilden för version 9.0 som bas
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build

# Sätt arbetskatalogen
WORKDIR /app

# Kopiera projektfilen och återställ beroenden
COPY *.csproj ./
RUN dotnet restore

# Kopiera all kod och bygg applikationen
COPY . ./
RUN dotnet publish -c Release -o out

# Använd runtime-bilden för .NET 9.0 för att köra applikationen
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/out ./

# Exponera porten som appen kör på
EXPOSE 80
CMD ["dotnet", "kit-backend.dll"]
