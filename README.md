# TaskFlow: PDF Generation Job Service

A scalable, independent backend service for generating PDFs from HTML templates asynchronously using Puppeteer Sharp, Redis, and MongoDB.

## Architecture

```
Client App
   |
   | POST /jobs/pdf
   v
TaskFlow.Api  ─────────►  Redis Queue
   |                           |
   | Store Job                 | Consume
   v                           v
MongoDB  ◄──────────────  TaskFlow.Worker
   |                           |
   | Fetch Job                 | Generate PDF
   v                           v
Local Storage  ◄───────────────┘
(Serve via API)
```

## Tech Stack

- **API**: ASP.NET Core 8 Web API
- **Worker**: .NET Worker Service
- **PDF Engine**: Puppeteer Sharp (Headless Chrome)
- **Queue**: Redis
- **Database**: MongoDB
- **Storage**: Local File Storage
- **Container**: Docker & Docker Compose

## Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (only for local development)

### 1. Configure Environment

Copy `.env.example` to `.env` and update the values:

```env
# MongoDB
MongoDB__ConnectionString=mongodb://admin:changeme123@localhost:27017?authSource=admin
MongoDB__DatabaseName=taskflow

# Redis
Redis__Connection=localhost:6379

# Local Storage
Storage__Path=./wwwroot/pdfs
Storage__BaseUrl=http://localhost:5095/pdfs/
```

### 2. Run with Docker Compose

```bash
# Build and start all services
docker-compose up --build -d
```
The API will be available at `http://localhost:5000`.

### 3. Test the API

**Create a PDF Job:**
```bash
curl -X POST http://localhost:5000/jobs/pdf \
  -H "Content-Type: application/json" \
  -d '{
    "templateHtml": "<html><body><h1>Hello World</h1></body></html>",
    "metadata": {
      "sourceApp": "test",
      "referenceId": "123"
    }
  }'
```

**Check Job Status:**
```bash
curl http://localhost:5000/jobs/{jobId}
```

## API Reference

### POST `/jobs/pdf`
Submit a new PDF generation request.

**Request Body:**
```json
{
  "templateHtml": "<html>...</html>",
  "data": { "key": "value" },
  "metadata": {
    "sourceApp": "billing",
    "referenceId": "INV-1001"
  }
}
```

### GET `/jobs/{id}`
Retrieve the status and link to the generated PDF.

**Response:**
```json
{
  "jobId": "6791bd82ef4b0d4f2f9ba76d",
  "status": "Completed",
  "fileUrl": "http://localhost:5000/pdfs/job_6791bd82ef4b0d4f2f9ba76d_20260122.pdf",
  "errorMessage": null
}
```

## Project Structure

- `TaskFlow.Domain`: Core entities and enumerations.
- `TaskFlow.Application`: DTOs and service interfaces.
- `TaskFlow.Infrastructure`: Implementation of storage, PDF generation, and persistence.
- `TaskFlow.Api`: REST API for job management and serving files.
- `TaskFlow.Worker`: Background service that processes the PDF queue.

## License
MIT
