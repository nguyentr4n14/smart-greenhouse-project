# Smart Greenhouse Project

A full-stack IoT monitoring system for smart greenhouse management with ASP.NET Core backend and React frontend.

## Team Members

-   Xuan Khoi Nguyen Tran
-   Huy Tran
-   Vladimir Rybin
-   Channa Perera

## Features

### Assignment 2: Core System

-   Device management (create, list, update, delete)
-   Sensor reading capture and storage
-   Historical data querying
-   PostgreSQL database with EF Core migrations
-   RESTful API with Swagger documentation

### Assignment 3: Design Patterns

-   **Observer Pattern**: Event-driven alert system
    -   Automatic alert rule evaluation on new readings
    -   Console logging of all sensor events
    -   Alert notifications with timestamps
-   **Strategy Pattern**: Flexible control policies
    -   Hysteresis cooling strategy for temperature control
    -   Moisture top-up strategy for irrigation
    -   Runtime strategy selection per device
    -   Configurable parameters via JSON

## Quick Start with Docker

### Prerequisites

-   Docker and Docker Compose installed
-   .NET 8 SDK (for backend development)
-   Node.js 18+ (for frontend development)
-   Git (to clone the repository)

### 1. Start PostgreSQL Database

The application uses PostgreSQL with a dedicated user. Start the database using Docker:

```bash
# Start PostgreSQL container
docker run --name greenhouse-db -d \
  -e POSTGRES_DB=greenhouse \
  -e POSTGRES_USER=greenhouse_user \
  -e POSTGRES_PASSWORD=greenhouse123 \
  -p 5432:5432 \
  postgres:16

# Verify the container is running
docker ps
```

**Note:** If you have another PostgreSQL instance running on port 5432, either stop it or use a different port (e.g., `-p 5433:5432`) and update the connection string accordingly.

### 2. Run Backend API

```bash
# Navigate to backend directory
cd backend/src/SmartGreenhouse.Api

# Run Entity Framework migrations
dotnet ef database update -p ../../src/SmartGreenhouse.Infrastructure

# Start the API server
dotnet run
```

The API will be available at **http://localhost:5080** with Swagger UI at **http://localhost:5080/swagger**

### 3. Run Frontend

```bash
# Navigate to frontend directory
cd frontend/smart-greenhouse-web

# Install dependencies
npm install

# Start development server
npm run dev
```

The frontend will be available at **http://localhost:5173**

## Database Configuration

The application connects to PostgreSQL using these credentials:

-   **Host:** localhost
-   **Port:** 5432
-   **Database:** greenhouse
-   **Username:** greenhouse_user
-   **Password:** greenhouse123

Connection strings are configured in:

-   `backend/src/SmartGreenhouse.Api/appsettings.json` (production)
-   `backend/src/SmartGreenhouse.Api/appsettings.Development.json` (development)

## Database Population

To populate the database with sample data, run these curl commands (make sure the backend API is running first):

### 1. Create Sample Devices

```bash
# Create Device 1 - Main Greenhouse Controller
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "device": {
      "deviceName": "Main Greenhouse Controller",
      "deviceType": 0
    }
  }'

# Create Device 2 - Seedling Bay Monitor
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "device": {
      "deviceName": "Seedling Bay Monitor",
      "deviceType": 0
    }
  }'

# Create Device 3 - Hydroponic System
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "device": {
      "deviceName": "Hydroponic System",
      "deviceType": 0
    }
  }'

# Create Device 4 - External Weather Station
curl -X POST "http://localhost:5080/api/devices" \
  -H "Content-Type: application/json" \
  -d '{
    "device": {
      "deviceName": "External Weather Station",
      "deviceType": 0
    }
  }'
```

### 2. Capture Temperature Readings

```bash
# Temperature readings for each device (SensorType: Temperature = 0)
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 0}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": 0}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": 0}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": 0}'
```

### 3. Capture Humidity Readings

```bash
# Humidity readings for each device (SensorType: Humidity = 1)
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 1}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": 1}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": 1}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": 1}'
```

### 4. Capture Light Readings

```bash
# Light readings for each device (SensorType: Light = 2)
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 2}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": 2}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": 2}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 4, "sensorType": 2}'
```

### 5. Capture Soil Moisture Readings

```bash
# Soil moisture readings for devices with soil sensors (SensorType: SoilMoisture = 3)
curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 3}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 2, "sensorType": 3}'

curl -X POST "http://localhost:5080/api/readings/capture" \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 3, "sensorType": 3}'
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
curl -s "http://localhost:5080/api/readings?sensorType=0" | jq .

# Check readings for specific device and sensor type
curl -s "http://localhost:5080/api/readings?deviceId=1&sensorType=0" | jq .
```

## API Endpoints

### Devices

-   `GET /api/devices` - List all devices
-   `POST /api/devices` - Create a new device
-   `GET /api/devices/{id}` - Get device by ID
-   `PUT /api/devices/{id}` - Update device
-   `DELETE /api/devices/{id}` - Delete device

### Readings

-   `GET /api/readings` - Query sensor readings (with filters)
-   `POST /api/readings/capture` - Capture a new sensor reading

### Alert Rules (Assignment 3)

-   `GET /api/alertrules` - List alert rules
-   `POST /api/alertrules` - Create an alert rule
-   `DELETE /api/alertrules/{id}` - Delete an alert rule

### Alerts (Assignment 3)

-   `GET /api/alerts` - List triggered alert notifications

### Control (Assignment 3)

-   `POST /api/control/profile` - Set control strategy for a device
-   `GET /api/control/profile/{deviceId}` - Get control profile
-   `POST /api/control/evaluate` - Evaluate control strategy and get actuator commands

## Testing the System

### Example: Create a Device and Alert Rule

```bash
# 1. Create a device
curl -X POST http://localhost:5080/api/devices \
  -H "Content-Type: application/json" \
  -d '{"device": {"deviceName": "Greenhouse Zone A", "deviceType": 0}}'

# 2. Create an alert rule (trigger if temperature > 26)
curl -X POST http://localhost:5080/api/alertrules \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": "Temperature", "operatorSymbol": ">", "threshold": 26, "isActive": true}'

# 3. Capture a temperature reading
curl -X POST http://localhost:5080/api/readings/capture \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 0}'

# 4. Check triggered alerts
curl http://localhost:5080/api/alerts?deviceId=1
```

### Example: Configure Control Strategy

```bash
# 1. Set hysteresis cooling strategy
curl -X POST http://localhost:5080/api/control/profile \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "strategyKey": "HysteresisCooling", "parameters": {"onAbove": 26, "offBelow": 24}}'

# 2. Capture sensor readings
curl -X POST http://localhost:5080/api/readings/capture \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1, "sensorType": 0}'

# 3. Evaluate control (get actuator commands)
curl -X POST http://localhost:5080/api/control/evaluate \
  -H "Content-Type: application/json" \
  -d '{"deviceId": 1}'
```

## Project Structure

```
.
├── backend/
│   ├── SmartGreenhouse.sln
│   └── src/
│       ├── SmartGreenhouse.Api/          # REST API & Controllers
│       ├── SmartGreenhouse.Application/  # Business Logic & Services
│       │   ├── Abstractions/             # Interfaces
│       │   ├── Control/                  # Strategy Pattern
│       │   ├── DeviceIntegration/        # Device abstraction
│       │   ├── Events/                   # Observer Pattern
│       │   └── Services/                 # Application services
│       ├── SmartGreenhouse.Domain/       # Entities & Enums
│       ├── SmartGreenhouse.Infrastructure/ # Database & EF Core
│       └── SmartGreenhouse.Shared/       # Shared types
└── frontend/
    └── smart-greenhouse-web/             # React + TypeScript UI
```

## Architecture Features

-   **Clean Architecture** with separation of concerns
-   **Abstract Factory Pattern** for device integrations
-   **Observer Pattern** for event-driven alerts (Assignment 3)
-   **Strategy Pattern** for flexible control policies (Assignment 3)
-   **Enum-based Type Safety** for sensor and device types
-   **Entity Framework Core** with PostgreSQL
-   **DTOs** for clean API contracts
-   **Dependency Injection** throughout the stack

## Design Patterns Implemented

### Observer Pattern (Assignment 3)

The Observer pattern enables automatic reactions to sensor readings:

-   **Publisher**: `ReadingPublisher` notifies all registered observers
-   **Observers**:
    -   `LogObserver` - Logs readings to console
    -   `AlertRuleObserver` - Evaluates alert rules and creates notifications
-   **Extensibility**: New observers can be added without modifying existing code

### Strategy Pattern (Assignment 3)

The Strategy pattern allows dynamic control algorithm selection:

-   **Strategies**:
    -   `HysteresisCoolingStrategy` - Temperature control with hysteresis
    -   `MoistureTopUpStrategy` - Irrigation control based on soil moisture
-   **Selector**: `ControlStrategySelector` chooses strategy based on device configuration
-   **Extensibility**: New strategies can be added by implementing `IControlStrategy`

## Sensor Types

The system supports these sensor types:

-   **Temperature** (0) - °C
-   **Humidity** (1) - %
-   **Light** (2) - lux
-   **SoilMoisture** (3) - %

## Device Types

-   **Simulated** (0) - For testing with generated random values
-   **MqttEdge** (1) - For future MQTT device integration

## Development

### Running Migrations

```bash
cd backend

# Create a new migration
dotnet ef migrations add MigrationName \
  -p src/SmartGreenhouse.Infrastructure \
  -s src/SmartGreenhouse.Api \
  -o Data/Migrations

# Apply migrations
dotnet ef database update \
  -p src/SmartGreenhouse.Infrastructure \
  -s src/SmartGreenhouse.Api
```

### Building the Solution

```bash
cd backend
dotnet build
```

### Running Tests

```bash
cd backend
dotnet test
```

## Troubleshooting

### Port 5432 Conflict

If you get authentication errors, check if another PostgreSQL instance is running:

```bash
netstat -ano | findstr :5432
```

Stop the conflicting service or use a different port in your Docker command and update the connection string.

### Migration Errors

If migrations fail, ensure:

1. PostgreSQL container is running: `docker ps`
2. Connection string matches your database credentials
3. Database is accessible: `docker exec -it greenhouse-db psql -U greenhouse_user -d greenhouse`

## Stopping Services

```bash
# Stop PostgreSQL container
docker stop greenhouse-db
docker rm greenhouse-db

# Stop backend (Ctrl+C in terminal)
# Stop frontend (Ctrl+C in terminal)
```

## License

This project is developed as part of an academic course at XAMK University of Applied Sciences.
