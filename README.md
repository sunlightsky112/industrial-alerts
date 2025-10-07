# 📘 Industrial Automation Alert Service

A full‑stack system that simulates industrial sensor telemetry, raises alerts when thresholds are exceeded, and provides a secure API with a Next.js dashboard.

---

## 📑 Table of Contents
1. [Overview](#overview)  
2. [Architecture](#architecture)  
3. [Tech Stack](#tech-stack)  
4. [Backend Setup](#backend-setup)  
5. [Frontend Setup](#frontend-setup)  
6. [Database Setup](#database-setup)  
7. [Running the System](#running-the-system)  
8. [Authentication](#authentication)  
9. [API Documentation](#api-documentation)  
10. [Frontend Features](#frontend-features)  
11. [Development Notes](#development-notes)  
12. [Testing](#testing)  
13. [Bonus Features](#bonus-features)  
14. [Folder Structure](#folder-structure)  
15. [Future Improvements](#future-improvements)  

---

## 📖 Overview
This project simulates an **Industrial Automation Alert Service**. It continuously generates random sensor readings (temperature and humidity), compares them against configurable thresholds, and raises alerts when values exceed limits.  

The system exposes secure APIs (JWT‑protected) and a **Next.js dashboard** where operators can:
- Log in
- View and update thresholds
- Monitor alerts
- Acknowledge alerts

---

## 🏗️ Architecture
- **Backend (C# / ASP.NET Core 8)**  
  - REST API with JWT authentication  
  - EF Core with PostgreSQL persistence  
  - BackgroundService simulating sensor data  
  - Alerts stored in DB and exposed via API  
  - Swagger for API docs  

- **Frontend (Next.js 14, App Router)**  
  - Login page with JWT handling  
  - Dashboard with config card and alerts table  
  - TanStack Query for data fetching  
  - TailwindCSS for styling  

- **Database (PostgreSQL)**  
  - `Configs` table: stores thresholds  
  - `Alerts` table: stores generated alerts  

---

## ⚙️ Tech Stack
- **Backend:** ASP.NET Core 8, EF Core, JWT, Swagger  
- **Frontend:** Next.js, TypeScript, TailwindCSS, TanStack Query  
- **Database:** PostgreSQL  
- **Dev Tools:** Docker (optional), Postman, VS Code REST Client  

---

## 🔧 Backend Setup
```bash
cd backend/Api
dotnet restore
```

### Configure connection string
In `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=alertsdb;Username=alerts_user;Password=alerts_pass"
}
```

### Apply migrations
```bash
dotnet ef database update --project Infrastructure --startup-project Api
```

### Run API
```bash
dotnet run --project Api
```
API available at: [http://localhost:5150/swagger](http://localhost:5150/swagger)

---

## 🎨 Frontend Setup
```bash
cd frontend
npm install
npm run dev
```
Frontend available at: [http://localhost:3000](http://localhost:3000)

---

## 🗄️ Database Setup
- Run PostgreSQL via Docker:
```bash
cd backend
docker-compose up -d
docker ps
docker exec -it <container_name_or_id> psql -U postgres
```

- Create a DB user and grant privillage
```bash
CREATE USER alerts_user WITH PASSWORD 'alerts_pass';
GRANT ALL PRIVILEGES ON DATABASE alertsdb TO alerts_user;
```

---

## ▶️ Running the System
1. Start PostgreSQL  
2. Run backend (`dotnet run --project Api`)  
3. Run frontend (`npm run dev`)  
4. Open [http://localhost:3000/login](http://localhost:3000/login)  

---

## 🔑 Authentication
- Demo credentials:
  - **Username:** `operator`  
  - **Password:** `password123`  
- `POST /auth/login` returns a JWT  
- Use JWT in `Authorization: Bearer <token>` header for all other endpoints  

---

## 📡 API Documentation

### Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST   | `/auth/login` | Authenticate and return JWT |
| GET    | `/config` | Get thresholds |
| PUT    | `/config` | Update thresholds |
| GET    | `/alerts?status=open|ack&from=...&to=...` | List alerts with filters |
| POST   | `/alerts/{id}/ack` | Acknowledge alert |

- Swagger UI: http://localhost:5150/swagger
- Postman collection: [docs/IndustrialAlerts.postman_collection.json](docs/IndustrialAlerts.postman_collection.json)
- VS Code REST Client: [docs/api.http](docs/api.http)

---

## 🖥️ Frontend Features
- **Login Page** → calls `/auth/login`, stores JWT, redirects to dashboard  
- **Dashboard**  
  - Config card → view/update thresholds  
  - Alerts table → list alerts, filter by status/date, acknowledge alerts  

---

## 🧑‍💻 Development Notes
- BackgroundService generates random readings every 3–5s  
- Alerts are stored in DB with `Status = Open` until acknowledged  
- All times stored in UTC  

---

## 🧪 Testing
- Unit tests for core logic in backend (`xUnit`)  
- Manual testing via Swagger, Postman, or `.http` file  
- Frontend tested via browser  

---

## ⭐ Bonus Features
- SignalR hub for live alerts (optional)  
- Docker Compose for backend + frontend + Postgres  
- GitHub Actions CI  

---

## 📂 Folder Structure
```
backend/
  Api/              # ASP.NET Core Web API
  Domain/           # Entities & enums
  Infrastructure/   # EF Core DbContext, services
frontend/
  app/              # Next.js App Router pages
  lib/              # API client
docs/
  IndustrialAlerts.postman_collection.json
  api.http
```

---

## 🚀 Future Improvements
- Add role‑based auth (admin/operator)  
- Add pagination to alerts  
- Add charts for telemetry trends  
- Add Docker Compose for one‑command startup  
