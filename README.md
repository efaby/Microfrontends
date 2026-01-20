# Microfrontends with Blazor WebAssembly and Cognito

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
| `/login`        | Initiates login with Cognito |
| `/logout`       | Logs out user |
| `/auth/me`      | Returns authenticated user |
| `/signin-oidc`  | OIDC callback |
| `/signout-callback-oidc` | Logout callback |

---

## 🧠 Authentication Flow

1. MFE redirects to `/login`
2. Cognito authenticates the user
3. Cognito returns to `Host`
4. Cookie is saved in the browser
5. MFEs query `/auth/me`
6. User available in all MFEs

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


---

## 🧪 Local Development

```bash
dotnet restore
dotnet build
dotnet run --project Host
```

Access to:

- http://localhost:5001

---

## 📦 Requisites

- .NET 10
- Docker / Docker Compose
- AWS account with Cognito configured


## 📄 License

MIT
