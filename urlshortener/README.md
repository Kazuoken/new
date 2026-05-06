# ✂️ Snip — URL Shortener

A production-ready URL shortener built with **.NET 9** (backend) and **React + Vite** (frontend), containerized with Docker, and deployed via **GitHub Actions → Docker Hub → Render**.

---

## 🏗️ Architecture

```
User → React SPA → .NET 9 API → SQLite DB
                              → In-Memory Cache
```

| Layer       | Technology                      |
|-------------|---------------------------------|
| Frontend    | React 18, Vite, CSS Modules     |
| Backend     | .NET 9, ASP.NET Core Web API    |
| Database    | SQLite via Entity Framework Core|
| Cache       | IMemoryCache (in-process)       |
| Container   | Docker (multi-stage builds)     |
| CI/CD       | GitHub Actions                  |
| Registry    | Docker Hub                      |
| Hosting     | Render (PaaS)                   |

---

## 📁 Project Structure

```
urlshortener/
├── backend/
│   ├── UrlShortener.API/
│   │   ├── Controllers/
│   │   │   ├── UrlController.cs       # CRUD API
│   │   │   └── RedirectController.cs  # Short link redirect
│   │   ├── Data/
│   │   │   └── AppDbContext.cs
│   │   ├── Migrations/
│   │   ├── Models/
│   │   │   ├── ShortenedUrl.cs
│   │   │   └── DTOs/UrlDTOs.cs
│   │   ├── Services/
│   │   │   ├── IUrlService.cs
│   │   │   └── UrlService.cs          # Business logic + Base62 encoding
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── App.jsx                    # Main UI
│   │   ├── App.module.css
│   │   ├── api.js                     # Axios service layer
│   │   └── main.jsx
│   ├── Dockerfile
│   └── package.json
├── .github/
│   └── workflows/
│       └── ci-cd.yml                  # Full CI/CD pipeline
├── docker-compose.yml
└── README.md
```

---

## 🚀 Quick Start

### Local development (Docker Compose)

```bash
git clone https://github.com/YOUR_USERNAME/urlshortener.git
cd urlshortener
docker compose up --build
```

- Frontend: http://localhost:5173  
- Backend API: http://localhost:8080  
- Swagger: http://localhost:8080/swagger

### Local development (without Docker)

**Backend:**
```bash
cd backend/UrlShortener.API
dotnet run
# API available at http://localhost:8080
```

**Frontend:**
```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

---

## 🔌 API Reference

| Method | Endpoint               | Description                  |
|--------|------------------------|------------------------------|
| POST   | `/api/url`             | Create a shortened URL       |
| GET    | `/api/url/manage`      | List all URLs (paginated)    |
| GET    | `/api/url/{id}`        | Get URL by ID                |
| PATCH  | `/api/url/{id}`        | Update URL (title/active)    |
| DELETE | `/api/url/{id}`        | Delete a URL                 |
| GET    | `/r/{code}`            | Redirect to original URL     |

### Create URL — Request body

```json
{
  "originalUrl": "https://example.com/very/long/path",
  "title": "My Link",
  "customCode": "my-brand",
  "expiresAt": "2025-12-31T00:00:00Z"
}
```

### URL Code Generation

Two strategies:
1. **Auto-generate**: Base62 encoding of ID + random salt → 6–7 char code (e.g., `a8Kz3m`)
2. **Custom code**: User-specified slug (e.g., `my-brand`)

---

## 🔄 CI/CD Pipeline

```
Push to main
    │
    ├─► build-backend  (dotnet restore → build → test)
    │
    ├─► build-frontend (npm ci → lint → build)
    │
    ├─► docker-publish (Build + push to Docker Hub with SHA and latest tags)
    │       backend:  yourname/urlshortener-api:latest
    │       frontend: yourname/urlshortener-frontend:latest
    │
    └─► deploy (Trigger Render deploy hooks via curl)
```

### Required GitHub Secrets

| Secret                        | Description                              |
|-------------------------------|------------------------------------------|
| `DOCKERHUB_USERNAME`          | Docker Hub username                      |
| `DOCKERHUB_TOKEN`             | Docker Hub access token                  |
| `RENDER_DEPLOY_HOOK_BACKEND`  | Render deploy hook URL for backend       |
| `RENDER_DEPLOY_HOOK_FRONTEND` | Render deploy hook URL for frontend      |
| `PROD_API_URL`                | Production API base URL for frontend     |

---

## ☁️ Deploying to Render

### Backend (Web Service)

1. Create a **Web Service** on Render
2. Set image: `yourname/urlshortener-api:latest`
3. Add environment variables:
   - `ASPNETCORE_URLS=http://+:8080`
   - `AllowedOrigins=https://your-frontend.onrender.com`
4. Add a **persistent disk** mounted at `/data` for SQLite
5. Copy the **Deploy Hook** URL → add as `RENDER_DEPLOY_HOOK_BACKEND` secret

### Frontend (Web Service)

1. Create a **Web Service** on Render
2. Set image: `yourname/urlshortener-frontend:latest`
3. Port: `80`
4. Copy the **Deploy Hook** URL → add as `RENDER_DEPLOY_HOOK_FRONTEND` secret

---

## 🧑‍💻 Key Design Decisions

- **SQLite + EF Core**: Zero-config relational DB, perfect for a PaaS with a mounted disk
- **IMemoryCache**: Hot-path caching for redirect lookups (code → URL)
- **Base62 encoding**: Compact, URL-safe codes from auto-incremented IDs
- **Multi-stage Docker builds**: Minimal final images (SDK not included at runtime)
- **CORS**: Configured per environment via `appsettings.json`
- **Fire-and-forget click tracking**: Redirect performance unaffected by DB write

---

## 📜 License

MIT
