# Patient Visit Manager - .NET Core Web API

## Overview

This is a comprehensive .NET Core Web API application for managing patient visits in a healthcare setting. The application implements clean architecture principles, repository pattern, dependency injection, and provides both REST API endpoints and a web-based frontend.

## Features

- **User Authentication & Authorization**: JWT-based authentication with role-based access control
- **Patient Management**: CRUD operations for patient records
- **Doctor Management**: Manage doctor profiles and specializations
- **Visit Scheduling**: Schedule and manage patient visits
- **Fee Schedule**: Manage service pricing
- **Activity Logging**: Track all system activities
- **Web Frontend**: HTML/CSS/JavaScript frontend for interacting with the API

## Architecture & Design Patterns

### SOLID Principles Implementation

1. **Single Responsibility Principle**: Each class has a single, well-defined responsibility
   - Controllers handle HTTP requests/responses
   - Services contain business logic
   - Repositories handle data access
   - Models represent data structures

2. **Open/Closed Principle**: Classes are open for extension but closed for modification
   - Interface-based design allows for easy extension
   - Dependency injection enables swapping implementations

3. **Liskov Substitution Principle**: Derived classes are substitutable for their base classes
   - Repository implementations can be substituted through interfaces

4. **Interface Segregation Principle**: Clients depend only on interfaces they use
   - Separate repository interfaces for each entity
   - Focused service interfaces

5. **Dependency Inversion Principle**: High-level modules don't depend on low-level modules
   - Controllers depend on service abstractions
   - Services depend on repository abstractions

### Design Patterns Used

1. **Repository Pattern**: Abstracts data access logic
2. **Factory Pattern**: `IDbConnectionFactory` for creating database connections
3. **Strategy Pattern**: Different authentication strategies can be implemented
4. **Middleware Pattern**: Custom middleware for exception handling and logging

### Dependency Injection Configuration

The application uses built-in .NET Core DI container:

```csharp
// Database
services.AddSingleton<IDbConnectionFactory>(SqlConnectionFactory);

// Repositories
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPatientRepository, PatientRepository>();
// ... other repositories

// Services  
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IPatientService, PatientService>();
// ... other services
```

## Technology Stack

- **.NET 8.0**: Latest LTS version
- **ADO.NET**: Direct database communication
- **SQL Server**: Database
- **JWT Authentication**: Secure token-based auth
- **Serilog**: Structured logging
- **FluentValidation**: Input validation
- **BCrypt**: Password hashing
- **Bootstrap 5**: Frontend UI framework

## Database Schema

The application uses SQL Server with the following main tables:

- `Users`: User authentication and authorization
- `Patients`: Patient information
- `Doctors`: Doctor profiles
- `Visits`: Patient visit records
- `FeeSchedule`: Service pricing
- `ActivityLogs`: System activity tracking

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Database Setup

1. Execute the DDL script to create the database schema:
   ```sql
   -- Run project-01-DDL.sql in SQL Server Management Studio
   ```

2. Execute the DML script to insert sample data:
   ```sql
   -- Run project-01-DML.sql for sample data
   ```

3. Execute the stored procedures script:
   ```sql
   -- Run project-01-SP.sql for stored procedures
   ```

### Application Setup

1. Clone the repository and navigate to the project folder
2. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PatientVisitManager;Trusted_Connection=true"
     }
   }
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Build the application:
   ```bash
   dotnet build
   ```

5. Run the application:
   ```bash
   dotnet run
   ```

6. Open your browser and navigate to `https://localhost:7xxx` (port will be displayed in console)

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration  
- `POST /api/auth/change-password` - Change password

### Patients
- `GET /api/patients` - Get all patients (with search and pagination)
- `GET /api/patients/{id}` - Get patient by ID
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient

### Doctors
- `GET /api/doctors` - Get all doctors
- `GET /api/doctors/{id}` - Get doctor by ID
- `POST /api/doctors` - Create new doctor
- `PUT /api/doctors/{id}` - Update doctor
- `DELETE /api/doctors/{id}` - Delete doctor

### Visits
- `GET /api/visits` - Get all visits
- `GET /api/visits/{id}` - Get visit by ID
- `POST /api/visits` - Schedule new visit
- `PUT /api/visits/{id}/status` - Update visit status
- `DELETE /api/visits/{id}` - Cancel visit

### Fee Schedule
- `GET /api/feeschedule` - Get all services
- `GET /api/feeschedule/{id}` - Get service by ID
- `POST /api/feeschedule` - Create new service
- `PUT /api/feeschedule/{id}` - Update service
- `DELETE /api/feeschedule/{id}` - Delete service

### Activity Logs
- `GET /api/activitylogs` - Get activity logs (with filtering and pagination)

## Security Features

### Authentication
- JWT tokens with configurable expiration
- Secure password hashing using BCrypt
- Role-based authorization (Admin, Doctor, Receptionist)

### Authorization Policies
- `RequireAdmin`: Admin-only access
- `RequireDoctorOrAdmin`: Doctor or Admin access
- `RequireReceptionistOrAdmin`: Receptionist or Admin access

### Security Headers
- HTTPS enforcement
- CORS configuration
- Request validation

## Middleware Components

### Exception Handling Middleware
- Catches unhandled exceptions
- Returns consistent error responses
- Logs errors for debugging

### Request Logging Middleware
- Logs HTTP requests and responses
- Tracks response times
- Records user information

### Validation Filter
- Validates incoming requests
- Returns structured validation errors
- Uses FluentValidation for complex scenarios

## Logging Configuration

The application uses Serilog for structured logging:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/application-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

Logs are written to:
- Console (for development)
- Daily rolling files in `logs/` directory

## Testing

### Postman Collection
Import the provided Postman collection (`PatientVisitManager.postman_collection.json`) to test all API endpoints.

### Manual Testing
1. Register a new user via the web interface
2. Login with the registered credentials
3. Test patient CRUD operations
4. Verify role-based access control
5. Check activity logging functionality

## Deployment

### IIS Deployment
1. Publish the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. Copy published files to IIS web directory
3. Configure IIS application pool for .NET 8.0
4. Update connection string for production database
5. Configure HTTPS certificates

### Production Considerations
- Use a proper SQL Server instance (not LocalDB)
- Configure connection string in production
- Set up proper logging (centralized logging service)
- Configure reverse proxy (nginx/IIS) if needed
- Implement health checks
- Set up monitoring and alerting

## Future Enhancements

1. **Email Notifications**: Send appointment confirmations
2. **Calendar Integration**: Sync with external calendar systems
3. **Reporting Dashboard**: Advanced analytics and reports
4. **Mobile App**: React Native or Flutter mobile client
5. **File Upload**: Support for medical documents and images
6. **Real-time Updates**: SignalR for real-time notifications
7. **Multi-tenant Support**: Support multiple healthcare facilities

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contact

For questions or support, please contact [your-email@example.com]
