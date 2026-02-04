# ProspectRankings

A full-stack web application for managing prospect rankings, built with ASP.NET Core and React.

## Tech Stack

- **Backend**: ASP.NET Core 8.0, Entity Framework Core, PostgreSQL
- **Frontend**: React 18, TypeScript, Vite, Mantine UI, Tailwind CSS, Redux Toolkit
- **Infrastructure**: Docker, Docker Compose, Nginx

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose
- (Optional for local dev) [.NET 8 SDK](https://dotnet.microsoft.com/download) and [Node.js 18+](https://nodejs.org/)

## Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone https://github.com/Branda22/ProspectRankings.git
   cd ProspectRankings
   ```

2. **Create environment file**
   ```bash
   cp .env.example .env
   ```

3. **Edit `.env` with secure values**
   ```env
   DB_PASSWORD=your_secure_database_password
   JWT_SECRET=your_jwt_secret_key_at_least_32_characters_long
   ```

4. **Build and run**
   ```bash
   docker-compose up --build
   ```

5. **Access the application**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger docs: http://localhost:5000/swagger

## Docker Commands

```bash
# Start all services
docker-compose up

# Start in detached mode (background)
docker-compose up -d

# Rebuild and start
docker-compose up --build

# Stop all services
docker-compose down

# Stop and remove volumes (clears database)
docker-compose down -v

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f database

# Restart a specific service
docker-compose restart backend

# Execute command in running container
docker-compose exec backend dotnet ef database update
```

## Local Development (without Docker)

### Database Setup

Start only PostgreSQL with Docker:
```bash
docker-compose up -d database
```

Or install PostgreSQL locally and create a database named `prospectdb`.

### Backend

```bash
cd backend
dotnet restore
dotnet run
```

The API will be available at http://localhost:5000

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app will be available at http://localhost:3000

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### Weather (Sample Protected Endpoint)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/weather` | Get weather forecast (requires auth) |

### Request Examples

**Register:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123", "firstName": "John", "lastName": "Doe"}'
```

**Login:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'
```

**Authenticated Request:**
```bash
curl http://localhost:5000/api/weather \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Project Structure

```
ProspectRankings/
├── backend/
│   ├── Controllers/      # API endpoints
│   ├── Models/           # Domain models and DTOs
│   ├── Services/         # Business logic
│   ├── Data/             # Database context
│   ├── Dockerfile
│   └── Program.cs
├── frontend/
│   ├── src/
│   │   ├── components/   # Reusable UI components
│   │   ├── pages/        # Page components
│   │   ├── services/     # API client
│   │   └── store/        # Redux state management
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
├── .env.example
└── README.md
```

## Database Migrations

```bash
# Create a new migration
cd backend
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Apply migrations in Docker
docker-compose exec backend dotnet ef database update
```

## Troubleshooting

**Port already in use:**
```bash
# Check what's using the port
lsof -i :5000
lsof -i :3000

# Kill the process or change ports in docker-compose.yml
```

**Database connection issues:**
```bash
# Check if database container is running
docker-compose ps

# View database logs
docker-compose logs database

# Reset database
docker-compose down -v
docker-compose up --build
```

**Container won't start:**
```bash
# View detailed logs
docker-compose logs --tail=50 backend

# Rebuild from scratch
docker-compose down
docker-compose build --no-cache
docker-compose up
```

## License

MIT
