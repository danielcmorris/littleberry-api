# LittleBerry API

A .NET 8 Web API for the PFSA (Portuguese Fraternal Society of America) library management system. Migrated from .NET Framework 4.8.1 to run on Linux containers for GCP Cloud Run deployment.

## Features

- **Library Catalog Management** - Browse, search, and manage book collections by subject
- **Reservation System** - Request and track book reservations
- **Shipment Tracking** - Manage book shipments between councils
- **Member Services** - Public-facing APIs for member registration and council information
- **Image Storage** - Azure Blob Storage integration for book cover images
- **Email Notifications** - AWS SES integration for transactional emails
- **Authentication** - Auth0 JWT-based authentication

## Tech Stack

- **.NET 8** - Cross-platform runtime
- **Entity Framework Core 7** - Database access (SQL Server)
- **Docker** - Containerization
- **Azure Blob Storage** - Image storage
- **AWS S3** - File storage
- **AWS SES** - Email service
- **Auth0** - Identity management

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (for containerized deployment)
- SQL Server database
- Azure Storage account (for images)
- AWS account (for S3/SES)
- Auth0 tenant

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/danielcmorris/littleberry-api.git
cd littleberry-api
```

### 2. Configure settings

Copy the template files and add your configuration:

```bash
cp LittleberryApi/appsettings.Development.json.template LittleberryApi/appsettings.Development.json
cp docker-compose.yml.template docker-compose.yml
```

Edit the files with your actual connection strings and API keys.

### 3. Run locally

```bash
cd LittleberryApi
dotnet run
```

The API will be available at `http://localhost:5000`

### 4. Run with Docker

```bash
docker compose up -d --build
```

The API will be available at `http://localhost:8090`

## Configuration

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__server18` | SQL Server connection string |
| `ConnectionStrings__BlobStorage` | Azure Blob Storage connection string |
| `Auth0__Authority` | Auth0 domain (e.g., `https://tenant.auth0.com/`) |
| `Auth0__Audience` | Auth0 API identifier |
| `Auth0__ClientId` | Auth0 client ID |
| `AWS__BucketName` | S3 bucket name |
| `AWS__AccessKey` | AWS access key ID |
| `AWS__SecretKey` | AWS secret access key |
| `AWS__SES__Server` | SES SMTP server |
| `AWS__SES__Port` | SES SMTP port (587) |
| `AWS__SES__User` | SES SMTP username |
| `AWS__SES__Password` | SES SMTP password |

## API Endpoints

### Catalog

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/catalog` | Get recent books |
| GET | `/api/catalog/{prefix}` | Get books by subject prefix |
| GET | `/api/catalog/{prefix}/{booknumber}` | Get specific book |
| GET | `/api/library/search?prefix=&author=&title=` | Search catalog |
| POST | `/api/catalog?sid={sessionId}` | Create new book |
| PUT | `/api/catalog?sid={sessionId}` | Update book |

### Library Routes (Legacy)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/library/subjects` | Get all subjects |
| GET | `/library/subjects/{prefix}` | Get subject by prefix |
| GET | `/library/catalog/{prefix}/{booknumber}/history` | Get book history |
| GET | `/library/request?sid={sessionId}` | Get open requests |
| POST | `/library/request?sid={sessionId}` | Create request |
| GET | `/library/account?sid={sessionId}` | Get accounts |
| GET | `/library/account/{id}?sid={sessionId}` | Get account by ID |
| GET | `/library/accounts/search/{type}?q=&sid=` | Search accounts |

### Public Website

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/website/council/{id}` | Get council info |
| POST | `/api/website/member` | Register member |
| POST | `/api/website/quoterequest` | Submit quote request |
| POST | `/api/website/festarequest` | Submit festa request |

### Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Health check |

## Deployment

### Docker Deployment

1. Build and push the image:
```bash
docker build -t littleberry-api .
docker tag littleberry-api your-registry/littleberry-api:latest
docker push your-registry/littleberry-api:latest
```

2. Deploy with docker-compose or your preferred orchestrator.

### GCP Cloud Run

```bash
gcloud run deploy littleberry-api \
  --image your-registry/littleberry-api:latest \
  --platform managed \
  --region us-west1 \
  --allow-unauthenticated
```

## Project Structure

```
LittleberryApi/
├── Controllers/           # API endpoints
│   ├── PublicWebsite/    # Public-facing APIs
│   └── ...
├── Data/                  # Database context
├── Models/               # Entity models
│   ├── DTOs/            # Data transfer objects
│   └── StoredProcResults/ # SP result mappings
├── Services/             # Business logic services
├── Program.cs            # Application entry point
└── appsettings.json      # Configuration
```

## License

Proprietary - Morris Development
