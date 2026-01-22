# Hybrid Development Environment (NGINX + Blazor WASM)

This document explains how to run a **hybrid development environment** where:

- Some Blazor WASM microfrontends run inside Docker
- One or more WASM apps run locally using `dotnet watch`
- NGINX routes traffic transparently

---

## Config Host network in your local

Review this line or add 

```bash
127.0.0.1 localhost
```

## Supported Scenarios

### Option A – Orders Local

- Orders → Local
- Shell, Products, Customers → Docker

### Option B – Orders + Products Local

- Orders → Local
- Products → Local
- Shell, Customers → Docker

---

## Architecture

```
Browser
   |
   v
NGINX Gateway (Docker)
   |
   +--> Shell WASM (Docker)
   +--> Products WASM (Docker or Local)
   +--> Orders WASM (Docker or Local)
   +--> Customers WASM (Docker)
   +--> Host/Auth (.NET + Cognito)
```

---

## Important NGINX Rules

- Templates must contain **ONLY `server {}`**
- No `http {}` or `events {}`
- Templates live in `/etc/nginx/templates`
- Environment variables are substituted at startup

---

## Base NGINX Template

```nginx
server {
  listen 80;
  server_name localhost;

  location / {
    proxy_pass http://shell:80/;
  }

  location /products/ {
    proxy_pass http://${PRODUCTS_UPSTREAM}/;
  }

  location /orders/ {
    proxy_pass http://${ORDERS_UPSTREAM}/;
  }

  location /customers/ {
    proxy_pass http://customers:80/;
  }

  location /auth/ {
    proxy_pass http://host:8080/auth/;
  }
}
```

---

For this action is necessary commnet in your docker-compose.dev.yml all order define 

## Option A – Orders Local

Update this file, add environment variables and comment all Orders config.

### docker-compose.dev.yml

```yaml
environment:
  ORDERS_UPSTREAM: host.docker.internal:6002
  PRODUCTS_UPSTREAM: products:80

...

#orders:
  #  build: 
  #    context: .
  #    dockerfile: Orders.Client/Dockerfile
  #  container_name: orders-wasm
  #  expose:
  #    - "80"
  ...
  #- orders
```

### Run

```bash
docker compose -f docker-compose.dev.yml up -d
cd Orders.Client
dotnet watch run --urls=http://localhost:6002
```

---

## Option B – Orders + Products Local

Update this file, add environment variables and comment all Orders adn Products config.

### docker-compose.dev.yml

```yaml
environment:
  ORDERS_UPSTREAM: host.docker.internal:6002
  PRODUCTS_UPSTREAM: host.docker.internal:6001

  #orders:
  #  build: 
  #    context: .
  #    dockerfile: Orders.Client/Dockerfile
  #  container_name: orders-wasm
  #  expose:
  #    - "80"

  #products:
  #  build: 
  #    context: .
  #    dockerfile: Products.Client/Dockerfile
  #  container_name: products-wasm
  #  expose:
  #    - "80"
  ...
  #- orders
  #- products

```

### Run

```bash
docker compose -f docker-compose.dev.yml up -d

cd Orders.Client
dotnet watch run --urls=http://localhost:6002

cd Products.Client
dotnet watch run --urls=http://localhost:6001
```

---

## Switching Back to Full Docker

1. Remove upstream overrides
2. Point NGINX to container services
3. Rebuild

```bash
docker compose up --build -d
```

---

## Debugging

```bash
docker exec -it nginx-gateway cat /etc/nginx/conf.d/default.conf
docker logs nginx-gateway
```

---

## Rules of Thumb

- One local WASM = one port
- Use `host.docker.internal`
- Restart NGINX after env changes

---

Happy hacking 🚀