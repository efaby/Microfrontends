# =========================
# BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copiar todo el repo
COPY . .

# publicar SOLO el Host
RUN dotnet publish Host/Host.csproj -c Release -o /app/publish

# =========================
# RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Host.dll"]
