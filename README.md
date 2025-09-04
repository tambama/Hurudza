# Hurudza - Agricultural Management System

## Overview

Hurudza is a comprehensive agricultural management system designed to streamline farm operations, crop management, and tillage services across Zimbabwe. Built with modern web technologies, it provides a robust platform for managing farms, fields, crops, and agricultural services with multi-tier architecture supporting both web and mobile applications.

## Features

### Core Functionality

#### 🌾 **Farm Management**
- Complete farm profile management with detailed information tracking
- Farm ownership and user access control
- Location tracking with GPS coordinates
- Land utilization metrics (size, arable land, cleared areas)
- Water resource management and irrigation tracking
- Equipment and infrastructure inventory
- Historical crop data and land management practices

#### 🌱 **Field Management**
- Individual field tracking within farms
- Field mapping with location coordinates
- Field-crop assignments and rotation planning
- Soil type and condition monitoring
- Field-specific cultivation history

#### 🌿 **Crop Management**
- Comprehensive crop database
- Field-crop relationship tracking
- Planting and harvesting schedules
- Crop rotation planning
- Yield tracking and reporting
- Crop statistics and analytics

#### 🚜 **Tillage Services**
- Tillage program management
- Service request and tracking system
- Provider and recipient farm coordination
- Service cost tracking
- Hectare-based service measurement
- Tillage type categorization (plowing, discing, harrowing, etc.)
- Completion status monitoring

#### 📊 **Reporting & Analytics**
- Real-time dashboards for crop statistics
- Tillage service reports
- Farm performance metrics
- Regional agricultural insights
- Excel report generation
- Interactive data visualizations with charts

### Administrative Features

#### 🏛️ **Hierarchical Structure**
- Province → District → Local Authority → Ward organization
- Regional and conference-based grouping
- Administrative boundary management

#### 👥 **User Management**
- Role-based access control (RBAC)
- Permission-based authorization
- JWT authentication
- User profile management
- Farm-user assignments
- Multi-farm access support

#### 📧 **Communication**
- SendGrid email integration for notifications
- SMS services (Clickatell and Esolutions)
- Automated alerts and reminders

### Technical Features

- **Multi-tenant Architecture**: Support for multiple organizations
- **Offline Capability**: Synchronization API for mobile/offline scenarios
- **GPS Integration**: Location tracking for farms and fields
- **Audit Trail**: Automatic tracking of data changes
- **API Versioning**: Backward compatibility support
- **Swagger Documentation**: Interactive API documentation

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens with ASP.NET Identity
- **API Documentation**: Swagger/OpenAPI
- **Logging**: Serilog with file and console sinks

### Frontend
- **Web Application**: Blazor WebAssembly
- **Mobile Application**: .NET MAUI
- **UI Components**: Syncfusion Blazor components
- **Charts**: AmCharts for data visualization
- **Maps**: Interactive mapping for farm/field locations

### Architecture
- **Pattern**: Clean Architecture with Domain-Driven Design
- **Data Access**: Repository pattern
- **Dependency Injection**: Built-in .NET DI container
- **Mapping**: AutoMapper for DTO transformations
- **Geographic Data**: NetTopologySuite

## Project Structure

```
hurudza/
├── Apis/                       # API Layer
│   ├── Hurudza.Apis.Core/     # Main API with controllers
│   ├── Hurudza.Apis.Base/     # Base infrastructure, filters, mapping
│   └── Hurudza.Apis.Sync/     # Synchronization API
├── Data/                       # Data Layer
│   ├── Hurudza.Data.Context/  # EF Core DbContext, migrations
│   ├── Hurudza.Data.Models/   # Domain entities
│   ├── Hurudza.Data.Repository/ # Repository implementations
│   ├── Hurudza.Data.Services/ # Business logic services
│   ├── Hurudza.Data.UI.Models/ # ViewModels and DTOs
│   └── Hurudza.Data.Enums/    # Shared enumerations
├── Common/                     # Common Layer
│   ├── Hurudza.Common.Utils/  # Utilities and extensions
│   ├── Hurudza.Common.Services/ # Cross-cutting services
│   ├── Hurudza.Common.Emails/ # Email services
│   └── Hurudza.Common.Sms/    # SMS services
└── UI/                         # UI Layer
    ├── Hurudza.UI.Web/         # Blazor WebAssembly app
    ├── Hurudza.UI.Mobile/      # .NET MAUI mobile app
    └── Hurudza.UI.Shared/      # Shared UI components
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- PostgreSQL 12 or later
- Visual Studio 2022 or Visual Studio Code
- Node.js (for frontend tooling)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/hurudza.git
   cd hurudza
   ```

2. **Configure database connection**
   Update the connection string in `Apis/Hurudza.Apis.Core/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=HurudzaDb;Username=your_username;Password=your_password"
     }
   }
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core
   ```

4. **Seed initial data (optional)**
   ```bash
   dotnet run --project Data/Hurudza.Data.Migrator
   ```

### Running the Application

#### API Server
```bash
# Run the Core API (default: https://localhost:7148)
dotnet run --project Apis/Hurudza.Apis.Core
```

#### Web Application
```bash
# Run the Blazor Web App
dotnet run --project UI/Hurudza.UI.Web
```

#### Mobile Application
```bash
# Build and run for Android
dotnet build -t:Run -f net8.0-android --project UI/Hurudza.UI.Mobile

# Build and run for iOS (Mac only)
dotnet build -t:Run -f net8.0-ios --project UI/Hurudza.UI.Mobile
```

## Development

### Building the Solution

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build Apis/Hurudza.Apis.Core/Hurudza.Apis.Core.csproj
```

### Running Tests

```bash
dotnet test
```

### Adding Migrations

```bash
dotnet ef migrations add <MigrationName> --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core
```

## API Documentation

Once the API is running, you can access the Swagger documentation at:
- `https://localhost:7148/swagger`

## Configuration

### Environment Variables

Key configuration settings can be overridden using environment variables:

- `ASPNETCORE_ENVIRONMENT`: Development, Staging, or Production
- `ConnectionStrings__DefaultConnection`: Database connection string
- `JWT__Secret`: JWT signing key
- `SendGrid__ApiKey`: SendGrid API key for emails
- `Clickatell__ApiKey`: Clickatell API key for SMS

### Application Settings

Main configuration files:
- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Development-specific settings
- `launchSettings.json`: Development server configuration

## Security

- JWT-based authentication with refresh tokens
- Role-based and permission-based authorization
- Secure password hashing with ASP.NET Identity
- HTTPS enforcement in production
- Input validation and sanitization
- SQL injection prevention through Entity Framework Core

## Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is proprietary software. All rights reserved.

## Support

For support, please contact the development team or raise an issue in the project repository.

## Acknowledgments

- Built with ❤️ for the agricultural community of Zimbabwe
- Special thanks to all farmers and agricultural organizations using the system
- Powered by modern .NET technologies and open-source libraries