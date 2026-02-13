# ParPar Website – Backend API

ASP.NET Core 8 Web API for the ParPar website. It serves portfolio items (with images) and site configuration, backed by PostgreSQL.

## Tech stack

- **.NET 8** – Web API
- **Entity Framework Core 9** – ORM and migrations
- **PostgreSQL** – Database (via Npgsql)
- **Mapster** – Object mapping
- **Swagger/OpenAPI** – API docs
- **Docker** – Container build and run

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) (local or remote)
- [Docker](https://www.docker.com/) (optional, for containerized run)

## Configuration

### Connection string

Set the PostgreSQL connection in `appsettings.json` or via environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<host>;Database=<db>;Port=5432;UserId=<user>;Password=<password>;SslMode=Require;"
  }
}
```

For production, prefer **User Secrets** or **environment variables** instead of storing secrets in `appsettings.json`.

### CORS

The API allows the frontend at `http://localhost:5173` by default. Adjust the CORS policy in `Program.cs` if your frontend runs on another origin.

## Running locally

From the solution root:

```bash
cd backend
dotnet restore
dotnet run --project src/ParParWebsite.Api.csproj
```

Or from the project folder:

```bash
cd backend/src
dotnet run
```

- API: **http://localhost:5000** (or the port in `launchSettings.json`)
- Swagger UI: **http://localhost:5000/swagger** (when running in Development)

### Database migrations

Apply migrations before or after starting the app:

```bash
cd backend/src
dotnet ef database update
```

Create a new migration after model changes:

```bash
dotnet ef migrations add <MigrationName>
```

## Running with Docker

Build and run from the **backend** directory (so the Dockerfile context can see `src/` and the project file):

```bash
cd backend
docker build -t parpar-api -f src/Dockerfile .
docker run -p 8080:8080 -e ConnectionStrings__DefaultConnection="<your-connection-string>" parpar-api
```

The image runs EF Core migrations on startup via `entrypoint.sh`, then starts the API on port **8080**.

## API overview

### Portfolio (`/Portfolio`)

| Method | Route        | Description                          |
|--------|--------------|--------------------------------------|
| POST   | `/Create`    | Create portfolio (multipart/form-data) |
| GET    | `/Get`       | List all portfolios                  |
| GET    | `/GetById`   | Get portfolio by `PortfolioId`       |
| PUT    | `/Update`    | Update portfolio (multipart/form-data) |
| DELETE | `/Delete`    | Delete portfolio by `PortfolioId`    |

Portfolio create/update accept large multipart requests (e.g. images); request size limits are configured for large uploads.

### Config (`/Config`)

| Method | Route     | Description              |
|--------|-----------|--------------------------|
| POST   | `/Create` | Create config (JSON body) |
| GET    | `/Get`    | List all configs         |
| PUT    | `/Update` | Update/delete config by `configId` |

### Static files

Uploaded files are served under `/uploads/` (static files middleware). CORS headers are set for the frontend origin on those paths.

## Project structure

```
backend/
├── src/
│   ├── Controllers/       # PortfolioController, ConfigController
│   ├── DTOs/              # Data transfer objects
│   ├── Helper/            # Utilities (e.g. StringHelper)
│   ├── Infrastructure/    # AppDbContext (EF Core)
│   ├── Middleware/        # GlobalExceptionHandler, DisableFormValueModelBinding
│   ├── Migrations/        # EF Core migrations
│   ├── Models/            # Portfolio, PortfolioImage, Config, enums
│   ├── Services/          # Business logic and file handling
│   ├── Program.cs
│   ├── Dockerfile
│   └── entrypoint.sh      # Migrations + app startup in container
├── .github/workflows/     # CI/CD (e.g. Azure Container App)
└── ParParWebsite.sln
```

## CI/CD

GitHub Actions workflows under `.github/workflows/` support building and deploying to **Azure** (e.g. Azure Container Registry and Azure Container App). Configure repository and Azure secrets as described in the workflow comments.

## License

Private / internal use unless otherwise specified.
