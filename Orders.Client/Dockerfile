FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update && \
    apt-get install -y python3 && \
    ln -s /usr/bin/python3 /usr/bin/python && \
    rm -rf /var/lib/apt/lists/*

COPY . .

RUN dotnet workload install wasm-tools
RUN dotnet restore Orders.Client/Orders.Client.csproj

RUN dotnet publish Orders.Client/Orders.Client.csproj \
    -c Release \
    -o /app/publish

# =========================
# NGINX
# =========================
FROM nginx:alpine

# 🔥 BORRAR configs default
RUN rm -rf /etc/nginx/conf.d/*

# 🧱 Limpiar html
RUN rm -rf /usr/share/nginx/html/*

# 📦 copiar artefactos
COPY --from=build /app/publish/wwwroot/orders/ /usr/share/nginx/html/orders/

# ✅ copiar TU config
COPY Orders.Client/nginx.conf /etc/nginx/nginx.conf

