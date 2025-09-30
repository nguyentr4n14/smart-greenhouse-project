# Smart Greenhouse Project

A full-stack IoT monitoring system for smart greenhouse management with ASP.NET Core backend and React frontend.

## Team Members

- Xuan Khoi Nguyen Tran
- Huy Tran
- Vladimir Rybin
- Channa Perera

## Quick Start with Docker

### Prerequisites

- Docker and Docker Compose installed
- Git (to clone the repository)

### 1. Start PostgreSQL Database

The application uses PostgreSQL with a dedicated user. Start the database using Docker:

```bash
# Start PostgreSQL container
docker run --name greenhouse-db -d \
  -e POSTGRES_DB=greenhouse \
  -e POSTGRES_USER=greenhouse_user \
  -e POSTGRES_PASSWORD=greenhouse123 \
  -p 5432:5432 \
  postgres:17

# Verify the container is running
docker ps
```

### 2. Run Backend API

```bash
# Navigate to backend directory
cd backend/src/SmartGreenhouse.Api

# Run Entity Framework migrations
dotnet ef database update -p ../SmartGreenhouse.Infrastructure

# Start the API server
dotnet run
```

The API will be available at `http://localhost:5080` with Swagger UI at `http://localhost:5080/swagger`

### 3. Run Frontend

```bash
# Navigate to frontend directory
cd frontend/smart-greenhouse-web

# Install dependencies
npm install

# Start development server
npm run dev
```

The frontend will be available at `http://localhost:5173`

## Database Configuration

The application connects to PostgreSQL using these credentials:

- **Host:** localhost
- **Port:** 5432
- **Database:** greenhouse
- **Username:** greenhouse_user
- **Password:** greenhouse123

## Database Population

To populate the database with sample data, run these curl commands (make sure the backend API is running first):

### 1. Create Sample Devices

```bash
# Create Device 1 - Main Greenhouse Controller
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Main Greenhouse Controller",
    "deviceType": "Simulated"
  }'

# Create Device 2 - Seedling Bay Monitor
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Seedling Bay Monitor",
    "deviceType": "Simulated"
  }'

# Create Device 3 - Hydroponic System
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "Hydroponic System",
    "deviceType": "Simulated"
  }'

# Create Device 4 - External Weather Station
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "deviceName": "External Weather Station",
    "deviceType": "Simulated"
  }'
```

### 2. Capture Temperature Readings

```bash
# Temperature readings for each device
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": "Temperature"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": "Temperature"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": "Temperature"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": "Temperature"}'
```

### 3. Capture Humidity Readings

```bash
# Humidity readings for each device
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": "Humidity"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": "Humidity"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": "Humidity"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": "Humidity"}'
```

### 4. Capture Light Readings

```bash
# Light readings for each device
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": "Light"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": "Light"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": "Light"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": "Light"}'
```

### 5. Capture Soil Moisture Readings

```bash
# Soil moisture readings for devices with soil sensors
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": "SoilMoisture"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": "SoilMoisture"}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": "SoilMoisture"}'
```

## Verification Commands

After populating the database, verify the data:

```bash
# Check all devices
curl -s "http://localhost:5080/api/devices" | jq .

# Check all readings
curl -s "http://localhost:5080/api/readings" | jq .

# Check readings for specific device
curl -s "http://localhost:5080/api/readings?deviceId=1" | jq .

# Check specific sensor type readings
curl -s "http://localhost:5080/api/readings?sensorType=Temperature" | jq .

# Check readings for specific device and sensor type
curl -s "http://localhost:5080/api/readings?deviceId=1&sensorType=Temperature" | jq .
```

## Project Structure

```text
├── backend/
│   └── src/
│       ├── SmartGreenhouse.Api/          # Web API controllers & DTOs
│       ├── SmartGreenhouse.Application/   # Business logic & services
│       ├── SmartGreenhouse.Domain/        # Entities & enums
│       └── SmartGreenhouse.Infrastructure/ # Data access & EF Core
└── frontend/
    └── smart-greenhouse-web/              # React + TypeScript + Tailwind CSS
```

## Architecture Features

- **Clean Architecture** with separation of concerns
- **Abstract Factory Pattern** for device integrations
- **Enum-based Type Safety** for sensor and device types
- **Entity Framework Core** with PostgreSQL
- **DTOs** for clean API contracts
- **Dependency Injection** throughout the stack

## Sensor Types

The system supports these sensor types:

- **Temperature** (°C)
- **Humidity** (%)
- **Light** (lux)
- **SoilMoisture** (%)

## Device Types

- **Simulated** - For testing with generated random values
- **MqttEdge** - For future MQTT device integration

## Stopping Services

```bash
# Stop PostgreSQL container
docker stop greenhouse-db
docker rm greenhouse-db

# Stop backend (Ctrl+C in terminal)
# Stop frontend (Ctrl+C in terminal)
```
