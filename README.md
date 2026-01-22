# Microfrontends with Blazor WebAssembly and Cognito

<img width="1503" height="800" alt="Screenshot 2026-01-22 at 4 15 17 PM" src="https://github.com/user-attachments/assets/1c95088e-6e5a-4b2c-a753-218c7edf417d" />



This repository demonstrates a **Microfrontend architecture using Blazor WebAssembly**, secured with **AWS Cognito (OpenID Connect)** and deployed using **Docker and NGINX**.

The system is designed with a **single authentication host** and multiple **independently deployable microfrontends**.

---

## 🏗 Architecture Overview

```
├── Host                # ASP.NET Core (Auth, Cognito, Cookies, Routing)
│
├── Shell.Client        # Blazor WASM (Main Shell)
├── Products.Client     # Blazor WASM (Products MFE)
├── Orders.Client       # Blazor WASM (Orders MFE)
│
├── Shared              # Shared UI, Contracts, Auth Client
└── nginx               # Reverse proxy (optional for prod)
```

### Key Concepts
- **Host** is the ONLY project that knows about Cognito
- Microfrontends consume authentication via `/auth/me`
- Cookies are shared across MFEs
- Each MFE can run in its **own container**

---

## 🔐 Authentication Flow

1. User accesses `/login`
2. Host redirects to **AWS Cognito**
3. Cognito redirects back to Host
4. Host issues secure authentication cookie
5. MFEs call `/auth/me` to retrieve user info

### Endpoints

| Endpoint        | Description |
|----------------|------------|
| `/auth/login`        | Initiates login with Cognito |
| `/auth/logout`       | Logs out user |
| `/auth/me`      | Returns authenticated user |
| `/signin-oidc`  | OIDC callback |
| `/signout-callback-oidc` | Logout callback |

---

## ⚙️ Configuration via Environment Variables

All sensitive configuration is injected via environment variables.

### Required Variables

```
Auth__Authority=https://cognito-idp.<region>.amazonaws.com/<user-pool-id>
Auth__ClientId=xxxxxxxx
Auth__ClientSecret=xxxxxxxx
```

ASP.NET Core automatically maps these to:

```
Auth:Authority
Auth:ClientId
Auth:ClientSecret
```

---

## 🐳 Running with Docker Compose

```bash
docker-compose up --build
```

Example `docker-compose.yml` snippet for Host:

```yaml
environment:
  Auth__Authority: https://cognito-idp.us-east-2.amazonaws.com/xxxx
  Auth__ClientId: ${COGNITO_CLIENT_ID}
  Auth__ClientSecret: ${COGNITO_CLIENT_SECRET}
```

Use a `.env` file (not committed):

```env
COGNITO_CLIENT_ID=xxxx
COGNITO_CLIENT_SECRET=xxxx
```

---

## 🧪 Local Development

This repository hosts a **microfrontend platform** built with:

- Blazor WebAssembly (Shell + MFEs)
- .NET Host for Authentication (Cognito + Cookies)
- NGINX as a Gateway / Router
- Docker for local and production environments

---

### Development Modes

### Hybrid Development (Recommended)

You can run some WASM microfrontends **locally with `dotnet watch`** while others run in Docker.
This enables very fast inner-loop development.

➡️ **Read the full guide here:**  
[`docs/dev-hybrid.md`](docs/dev-hybrid.md)

Supported hybrid scenarios:
- Orders running locally
- Orders + Products running locally
- All other MFEs in Docker

---

### Quick Start (Hybrid)

```bash
docker compose -f docker-compose.dev.yml up -d
cd Orders.Client
dotnet watch run
```

Open:

```
http://localhost
```

---

### Documentation

- `docs/dev-hybrid.md` – Hybrid development with NGINX + Docker
- `docker-compose.dev.yml` – Development environment
- `nginx.dev.conf.template` – NGINX routing template

---

# 🛠️ Add new Blazor WASM

[Click here to view the Host Configuration Guide](./Host/README.md)

## 📄 License

MIT
