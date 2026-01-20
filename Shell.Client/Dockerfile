
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 🔑 Instalar dependencias necesarias para WASM
RUN apt-get update && \
    apt-get install -y python3 python3-pip && \
    ln -s /usr/bin/python3 /usr/bin/python && \
    rm -rf /var/lib/apt/lists/*

# 🔑 Copiar TODO el repo (incluye Shared.Auth)
COPY . .

# 🔑 Instalar workload WASM
RUN dotnet workload install wasm-tools

# 🔑 Restaurar desde el proyecto correcto
RUN dotnet restore Shell.Client/Shell.Client.csproj

RUN dotnet publish Shell.Client/Shell.Client.csproj \
    -c Release \
    -o /app

FROM nginx:alpine
COPY --from=build /app/wwwroot /usr/share/nginx/html

COPY Shell.Client/nginx.conf /etc/nginx/nginx.conf
