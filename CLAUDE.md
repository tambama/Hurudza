# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Hurudza is a multi-tier agricultural management system built with:
- **Backend**: ASP.NET Core 8.0 Web API with PostgreSQL database
- **Frontend**: Blazor WebAssembly for web, .NET MAUI for mobile
- **Architecture**: Clean Architecture with separated layers for Data, APIs, Common utilities, and UI

## Development Commands

### Building the Solution
```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build Apis/Hurudza.Apis.Core/Hurudza.Apis.Core.csproj
dotnet build UI/Hurudza.UI.Web/Hurudza.UI.Web.csproj
```

### Running Applications
```bash
# Run Core API (default: https://localhost:7148)
dotnet run --project Apis/Hurudza.Apis.Core

# Run Blazor Web App (check launchSettings.json for ports)
dotnet run --project UI/Hurudza.UI.Web

# Run MAUI Mobile App
dotnet build -t:Run -f net8.0-android --project UI/Hurudza.UI.Mobile
```

### Database Operations
```bash
# Add migration
dotnet ef migrations add <MigrationName> --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core

# Update database
dotnet ef database update --project Data/Hurudza.Data.Context --startup-project Apis/Hurudza.Apis.Core

# Run data migrator/seeder
dotnet run --project Data/Hurudza.Data.Migrator
```

### Testing
```bash
# Run tests (when available)
dotnet test
```

## Architecture Overview

### Solution Structure
The solution follows Domain-Driven Design principles with clear separation of concerns:

1. **APIs Layer** (`/Apis`)
   - `Hurudza.Apis.Core`: Main API with authentication, controllers for farms, fields, crops, tillage services
   - `Hurudza.Apis.Base`: Base API infrastructure, filters, mapping profiles
   - `Hurudza.Apis.Sync`: Synchronization API for mobile/offline scenarios

2. **Data Layer** (`/Data`)
   - `Hurudza.Data.Context`: EF Core DbContext, migrations, seed data
   - `Hurudza.Data.Models`: Domain entities (Farm, Field, Crop, User, etc.)
   - `Hurudza.Data.Repository`: Repository pattern implementation
   - `Hurudza.Data.Services`: Business logic services
   - `Hurudza.Data.UI.Models`: ViewModels and DTOs
   - `Hurudza.Data.Enums`: Shared enumerations

3. **Common Layer** (`/Common`)
   - `Hurudza.Common.Utils`: Extensions, helpers, exceptions
   - `Hurudza.Common.Services`: Cross-cutting services (DateTime, CurrentUser)
   - `Hurudza.Common.Emails`: SendGrid email integration
   - `Hurudza.Common.Sms`: SMS services (Clickatell, Esolutions)

4. **UI Layer** (`/UI`)
   - `Hurudza.UI.Web`: Blazor WebAssembly application
   - `Hurudza.UI.Mobile`: .NET MAUI mobile application
   - `Hurudza.UI.Shared`: Shared UI components and services

### Key Architectural Patterns

1. **Authentication & Authorization**
   - JWT Bearer token authentication
   - Role-based and permission-based authorization
   - Custom policies defined in `AuthorizationPolicyConfiguration.cs`

2. **Database Access**
   - Entity Framework Core with PostgreSQL
   - Repository pattern for data access
   - Automatic audit fields (CreatedDate, ModifiedBy, etc.)

3. **API Versioning**
   - Configured with Asp.Versioning
   - Default version 1.0
   - Swagger documentation per version

4. **Dependency Injection**
   - Scoped services for per-request lifetime
   - Transient services for stateless operations
   - Singleton for shared resources

### Core Domain Concepts

- **Farms & Fields**: Central entities for agricultural management
- **Crops & FieldCrops**: Crop tracking and field assignments
- **Tillage Services**: Agricultural service management
- **Administrative Structure**: Province → District → LocalAuthority → Ward hierarchy
- **User Management**: Identity-based with roles and permissions
- **Location Tracking**: GPS coordinates for farms and fields

### Important Configuration Files

- `appsettings.json`: Connection strings, JWT settings, API URLs
- `launchSettings.json`: Development server configurations
- `global.json`: .NET SDK configuration

### External Dependencies

- **Mapping**: NetTopologySuite for geographic data
- **Excel**: EPPlus for report generation
- **Charts**: Syncfusion Blazor components
- **Logging**: Serilog with file and console sinks
- **Email**: SendGrid for transactional emails
- **SMS**: Clickatell and Esolutions providers