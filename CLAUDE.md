# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture

Full-stack web application:
- **Backend**: ASP.NET Core 10.0 Web API with JWT authentication, EF Core 10
- **Frontend**: React 18 + TypeScript + Vite + Mantine UI + Tailwind CSS + Redux Toolkit (PWA)
- **Database**: PostgreSQL 15
- **Infrastructure**: Docker & Docker Compose

## Project Structure

```
backend/              # C# .NET API
  Controllers/        # API endpoints (AuthController, etc.)
  Models/             # Domain models and DTOs
  Services/           # Business logic (AuthService, etc.)
  Data/               # EF Core DbContext and migrations

frontend/             # React PWA
  src/
    components/       # Reusable components (Layout)
    pages/            # Page components (Home, Login)
    services/         # API client (axios)
    store/            # Redux store and slices
```

## Development Commands

### Backend
```bash
cd backend
dotnet restore                       # Install dependencies
dotnet run                           # Run dev server (port 5000)
dotnet test                          # Run tests
dotnet ef migrations add <Name>      # Create migration
dotnet ef database update            # Apply migrations
```

### Frontend
```bash
cd frontend
npm install              # Install dependencies
npm run dev              # Run dev server (port 3000)
npm test                 # Run tests
npm run build            # Production build
npm run lint             # Run ESLint
```

### Docker (Full Stack)
```bash
docker-compose up --build    # Build and run all services
docker-compose up -d         # Run detached
docker-compose down          # Stop all services
docker-compose logs -f       # View logs
```

## Environment Variables

Copy `.env.example` to `.env` and configure:
- `DB_PASSWORD` - PostgreSQL password
- `JWT_SECRET` - JWT signing key (min 32 characters)
- `VITE_API_URL` - Backend API URL (default: /api via proxy)
