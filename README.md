# Product Catalog Management System (RESTful Web API)

A production-ready RESTful Web API engineered with **ASP.NET Core (.NET 8)** following **Clean Architecture** and **Domain-Driven Design (DDD)** principles. The application incorporates role-based access control (RBAC), URL segment API versioning, structured logging via Serilog, response compression, global error handling conforming to RFC 7807, and automated tests across unit, integration, and persistence layers.

---

## Architectural Principles & Structure

The solution strictly adheres to **Clean Architecture** patterns, ensuring loose coupling, separation of concerns, and independence from external frameworks:

```text

ProductCatalog-Assessment/
│
├── .gitignore
├── README.md
├── ProductCatalog-Assessment.sln
│
├── src/
│   ├── API/                                    # Presentation & Entry Point Layer
│   │   ├── Controllers/
│   │   │   │── EmployeesController.cs          # Employee CRUD & RBAC endpoints   
│   │   │   ├── AuthController.cs               # Login & Token refresh actions
│   │   │   └── ProductsController.cs           # Product & Item management endpoints
│   │   ├── Extensions/
│   │   │   ├── AuthenticationExtensions.cs     # AddJwtAuthentication setup & Bearer configurations
│   │   │   └── CorsExtensions.cs               # Cross-Origin Resource Sharing policy setup
│   │   ├── Filters/
│   │   │   └── ValidationFilters.cs          # ModelState validation filter for request payloads
│   │   ├── Logs/
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs  # RFC 7807 global error handling
│   │   │   └── SecurityHeadersMiddleware.cs    # Custom HTTP security headers
│   │   ├── appsettings.json                    # Connection strings, JWT configuration, Serilog configs
│   │   ├── appsettings.Development.json
│   │   ├── Program.cs                          # Application pipeline, Swagger, Versioning, Middleware
|   |   ├── DependencyInjection.cs              # API layer DI configuration (AddAPIDI)
│   │   └── API.csproj
│   │
│   ├── Application/                            # Business Logic & Workflows Layer
│   │   ├── AutoMapper/
│   │   │   └── AutoMapperConfig.cs             # DTO <-> Domain Entity mappings
│   │   ├── DTO/
│   │   │   ├── AuthDTO.cs                      # Credentials payload for authentication
│   │   │   ├── AuthResponseDTO.cs              # Access & Refresh token response
│   │   │   ├── CreateProductDTO.cs             # Product creation schema with nested items
│   │   │   ├── CreateItemDTO.cs                # Item creation schema with nested items
│   │   │   ├── EmployeeDTO.cs                  # Employee mutation payload
│   │   │   ├── EmpResponseDTO.cs               # Employee response projection
│   │   │   ├── ErrorResponseDTO.cs             # Error response projection
│   │   │   ├── ItemDTO.cs                      # Item  projection
│   │   │   ├── PagedResultDTO.cs               # Paged result projection
│   │   │   ├── ProductDTO.cs                   # Product response projection
│   │   │   ├── RefreshTokenRequestDTO.cs       # Token refresh request schema
│   │   │   ├── UpdateItemDTO.cs                # Item update payload
│   │   │   └── UpdateProductDTO.cs             # Product update payload
│   │   ├── Interfaces/
│   │   │   ├── IAppLogger.cs                   # Structured logger abstraction
│   │   │   ├── IAuthenticationService.cs       # Auth contract
│   │   │   ├── IEmployeeService.cs             # Employee business logic contract
│   │   │   ├── IGenerateJWTToken.cs            # Token generator contract
│   │   │   ├── IGenericRepository.cs           # Generic repository contract
│   │   │   ├── IProductRepository.cs           # Product data contract
│   │   │   ├── IProductService.cs              # Product business logic contract
│   │   │   └── IUnitOfWork.cs                  # Unit of Work transaction contract
│   │   ├── Services/
│   │   │   ├── AuthenticationService.cs        # Login, refresh, and password verification logic
│   │   │   ├── EmployeeService.cs              # Employee workflows & management
│   │   │   └── ProductService.cs               # Product & Item business workflows
│   │   ├── Validators/
│   │   │   └── CreateProductValidator.cs       # FluentValidation rules for product creation
│   │   ├── DependencyInjection.cs              # Application layer DI configuration 
│   │   └── API.Application.csproj
│   │
│   ├── Domain/                                 # Enterprise Core & Entities Layer
│   │   ├── Entities/
│   │   │   ├── Employee.cs                     # Employee entity (Id, Name, Email, PasswordHash, Role)
│   │   │   ├── Item.cs                         # Child Item entity associated with Products
│   │   │   └── Product.cs                      # Aggregate root Product entity
│   │   ├── Enum/
│   │   │   └── Roles.cs                        # RBAC roles definition (Admin, FrontDesk, User)
│   │   ├── Exceptions/
│   │   │   ├── ForbiddenAccessException.cs     # 403 Forbidden domain exception
│   │   │   ├── NotFoundException.cs            # 404 Entity not found exception
│   │   │   └── ValidationException.cs          # 400 Bad request validation failure
│   │   ├── Options/
│   │   │   └── ConnectionstringOptions.cs      # Connection string configuration options
│   │   ├── DependencyInjection.cs              # Domain layer DI configuration 
│   │   └── API.Domain.csproj
│   │
│   └── Infrastructure/                         # External Concerns & Persistence Layer
│       ├── Data/
│       │   ├── Config/                         # EntityTypeConfigurations (EF Core Fluent API)
│       │   │   ├── EmployeeConfig.cs           # Indexing, column constraints, Enum conversions
│       │   │   ├── ItemsConfig.cs              # Foreign key cascade constraints
│       │   │   └── ProductConfig.cs            # Column lengths, decimals, and constraints
│       │   ├── Repositories/
│       │   │   ├── GenericRepository.cs        # Base CRUD repository implementation
│       │   │   └── ProductRepository.cs        # Product-specific queries
│       │   ├── ApplicationDBContext.cs         # DbSets, OnModelCreating configurations
│       │   └── UnitOfWork.cs                   # SaveChangesAsync transaction orchestrator
│       ├── Identity/
│       │   └── GenerateJWTToken.cs             # Token generation, claims stamping, key signing
│       ├── Logging/
│       │   └── AppLogger.cs                    # Application structured logging implementation using Serilog
│       ├── Migrations/                         # Entity Framework Core database migrations
│       ├── DependencyInjection.cs              # Infrastructure layer DI configuration 
│       └── API.Infrastructure.csproj
│
└── tests/
    ├── API.Tests/                              # End-to-End Integration Tests
    │   ├── AuthTokenHelper.cs                  # Test JWT generation utility
    │   ├── CustomWebApplicationFactory.cs      # WebApplicationFactory host with In-Memory DB
    │   ├── ProductsControllerTests.cs          # Endpoint tests, 401 Unauthorized assertions
    │   └── API.Tests.csproj
    │
    ├── Application.Tests/                      # Service Layer Unit Tests
    │   ├── ProductServiceTests.cs              # Moq unit tests for Product business rules
    │   └── Application.Tests.csproj
    │
    └── Infrastructure.Tests/                   # Persistence Layer Tests
        ├── ProductRepositoryTests.cs           # EF Core InMemory database CRUD validation
        └── Infrastructure.Tests.csproj
            

``` bash
```

##  Layer Responsibilities

* Domain Layer: Encapsulates business models (Product, Item, Employee), domain enums (Roles: Admin, FrontDesk, User), and custom domain exceptions (NotFoundException, ForbiddenAccessException, ValidationException).
* Application Layer: Contains business rules, service interfaces (IProductService, IEmployeeService, IAuthenticationService), DTO mappings via AutoMapper, and repository interfaces (IUnitOfWork, IProductRepository, IGenericRepository<T>).
* Infrastructure Layer: Implements data access using Entity Framework Core 8 against SQL Server, repository implementations, Unit of Work, and JWT generation logic (GenerateJWTToken).
* API Layer: The HTTP host exposing versioned REST endpoints (/api/v1/...), Swagger/OpenAPI with XML documentation, JWT Bearer security integration, response compression, security headers, and global RFC 7807 problem details middleware.

## Technology Stack
* Framework: .NET 8.0 (C# 12)
* API Versioning: Asp.Versioning.Mvc & Asp.Versioning.Mvc.ApiExplorer (v1.0 URL segment format)
* Database & ORM: Microsoft SQL Server with Entity Framework Core 8
* Authentication & Identity: JWT Bearer tokens with refresh token rotation, Microsoft.AspNetCore.Identity.PasswordHasher<T>
* Authorization: Role-Based Access Control (Admin, FrontDesk, User)
* Object Mapping: AutoMapper 16.0.2
* Logging: Serilog structured logging with request interception middleware
* Documentation: Swagger / OpenAPI (Swashbuckle), Postman with JWT Bearer authorization
* Testing: xUnit, Moq, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)

## Database Schema Design
### SQL Table Definitions

```
-- 1. Products Table (Aggregate Root)
CREATE TABLE [dbo].[Products] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProductName] NVARCHAR(100) NOT NULL,
    [ProductDescription] NVARCHAR(250) NOT NULL,
    [ProductPrice] DECIMAL(18,2) NOT NULL,
    [CreatedBy] NVARCHAR(50) NOT NULL,
    [CreatedOn] DATETIME2 NOT NULL,
    [UpdatedBy] NVARCHAR(50) NULL,
    [UpdatedOn] DATETIME2 NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);

-- 2. Items Table (Child Entity)
CREATE TABLE [dbo].[Items] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT [PK_Items] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Items_Products] FOREIGN KEY ([ProductId]) 
        REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Items_ProductId] ON [dbo].[Items] ([ProductId]);

-- 3. Users Table (Employee Entity)
CREATE TABLE [dbo].[User] (
    [Id] INT IDENTITY(1001,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(500) NOT NULL,
    [Role] NVARCHAR(50) NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY ([Id])
);

CREATE UNIQUE INDEX [IX_User_Email] ON [dbo].[User] ([Email]);

```

## Getting Started & Local Execution
### Prerequisites
* .NET 8.0 SDK
* Microsoft SQL Server (LocalDB, Express, or standard edition)
* Visual Studio 2022 (v17.8+) or Visual Studio Code with the C# Dev Kit

## Authentication Flow in Swagger
* Call POST /api/v1/auth/login with your employee email and password.
* Copy the accessToken string from the JSON response.
* Click the Authorize button (green lock icon) at the top-right corner of the Swagger UI.
* Paste the token into the Value box and click Authorize.
* Requests to protected endpoints will automatically pass the Authorization: Bearer <token> header.

## 👨‍💻 Author

**Deepraj Borse**
