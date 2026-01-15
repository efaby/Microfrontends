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
RUN dotnet restore Products.Client/Products.Client.csproj

RUN dotnet publish Products.Client/Products.Client.csproj \
    -c Release \
    -o /app/publish

#=========================
# NGINX
# =========================
FROM nginx:alpine

# 🔥 BORRAR configs default
RUN rm -rf /etc/nginx/conf.d/*

# 🧱 Limpiar html
RUN rm -rf /usr/share/nginx/html/*

# 📦 copiar artefactos
COPY --from=build /app/publish/wwwroot/products/ /usr/share/nginx/html/products/

# ✅ copiar TU config
COPY Products.Client/nginx.conf /etc/nginx/nginx.conf