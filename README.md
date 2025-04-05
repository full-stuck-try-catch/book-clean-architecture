# Book Library Management System

A comprehensive library management system built with **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** patterns using **.NET 8**. This system allows users to manage libraries, books, loans, and reviews in a scalable and maintainable way.

## ??? Architecture Overview

This solution follows **Clean Architecture** principles with clear separation of concerns:

```
?? src/
??? ?? BookLibrary.Domain/          # Core business logic and entities
??? ?? BookLibrary.Application/     # Use cases and business workflows
??? ?? BookLibrary.Infrastructure/  # External concerns (Database, Auth, etc.)
??? ?? BookLibrary.Api/            # API controllers and presentation layer

?? test/
??? ?? BookLibrary.Domain.UnitTests/
??? ?? BookLibrary.Application.UnitTests/
??? ?? BookLibrary.Application.IntegrationTests/
??? ?? BookLibrary.Api.FunctionTests/
??? ?? BookLibrary.ArchitectureTests/
```

### Key Architectural Patterns

- **?? Clean Architecture**: Dependency inversion and separation of concerns
- **??? Domain-Driven Design (DDD)**: Rich domain models with business logic
- **?? CQRS**: Command Query Responsibility Segregation with MediatR
- **?? Repository Pattern**: Data access abstraction
- **?? Unit of Work**: Transaction management
- **?? Result Pattern**: Functional error handling

## ?? Technologies & Frameworks

### Backend Stack
- **.NET 8** - Modern C# application framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with PostgreSQL
- **MediatR** - CQRS and mediator pattern implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Quartz.NET** - Background job scheduling

### Infrastructure
- **PostgreSQL** - Primary database
- **Redis** - Caching and session storage
- **Docker & Docker Compose** - Containerization
- **Seq** - Log aggregation and analysis
- **JWT Bearer** - Authentication and authorization

### Testing
- **xUnit** - Unit testing framework
- **FluentAssertions** - Expressive assertions
- **NSubstitute** - Mocking framework
- **Testcontainers** - Integration testing with real databases
- **ASP.NET Core Testing** - API functional testing

## ?? Features

### ?? Core Functionality
- **Library Management**: Create and manage multiple libraries
- **Book Management**: Add, update, and organize books by library
- **User Management**: Registration, authentication, and profile management
- **Loan System**: Book borrowing and returning with due dates
- **Review System**: Users can review and rate books
- **Stock Management**: Track book availability and quantities

### ?? Security & Authentication
- **JWT Authentication**: Secure token-based authentication
- **Role-based Authorization**: Admin and User roles
- **Password Hashing**: BCrypt for secure password storage
- **API Versioning**: Support for multiple API versions

### ?? Monitoring & Logging
- **Health Checks**: Database and Redis connectivity monitoring
- **Structured Logging**: Comprehensive logging with Serilog
- **Request/Response Logging**: Detailed API interaction tracking
- **Error Handling**: Global exception handling middleware

## ??? Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)

### Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd book-clean-architecture
   ```

2. **Create Docker network**
   ```bash
   docker network create shared-net
   ```

3. **Start the application**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - **API**: http://localhost:5000
   - **Swagger UI**: http://localhost:5000/swagger
   - **Seq Logs**: http://localhost:5341 (admin/12345678)
   - **Database**: localhost:5432 (postgres/postgres)

### Development Setup

1. **Start infrastructure services**
   ```bash
   docker-compose up my-posgresql-db my-redis my-seq -d
   ```

2. **Update connection strings in `appsettings.Development.json`**
   ```json
   {
     "ConnectionStrings": {
       "Database": "Host=localhost;Database=BookLibraryDB;Username=postgres;Password=postgres;",
       "Cache": "localhost:6379"
     }
   }
   ```

3. **Run database migrations**
   ```bash
   cd src/BookLibrary.Api
   dotnet ef database update
   ```

4. **Start the API**
   ```bash
   dotnet run
   ```

## ?? API Documentation

### Authentication Endpoints
```http
POST /api/v1/users/register     # Register new user
POST /api/v1/users/login        # User login
GET  /api/v1/users/me          # Get current user profile
PUT  /api/v1/users/profile     # Update user profile
```

### Library Management
```http
POST /api/v1/libraries                    # Create library
POST /api/v1/libraries/{id}/books        # Add book to library
```

### Book Management
```http
POST /api/v1/books                       # Create book
GET  /api/v1/books/{id}                 # Get book details
GET  /api/v1/books/library/{libraryId}  # Get books by library
POST /api/v1/books/{id}/add-stock       # Add book stock
```

### Loan Management
```http
POST /api/v1/loans                      # Create loan
GET  /api/v1/loans/{id}                # Get loan details
GET  /api/v1/loans/my-loans           # Get current user's loans
POST /api/v1/loans/{id}/return        # Return book
POST /api/v1/loans/{id}/extend        # Extend loan period
```

### Book Operations (Users)
```http
POST /api/v1/users/borrow-book         # Borrow a book
POST /api/v1/users/return-book         # Return a book
```

### Review System
```http
POST /api/v1/reviews                   # Create review
GET  /api/v1/reviews/{id}             # Get review
PUT  /api/v1/reviews/{id}             # Update review
DELETE /api/v1/reviews/{id}           # Delete review
GET  /api/v1/reviews/books/{bookId}   # Get book reviews
```

## ?? Testing

### Run All Tests
```bash
dotnet test
```

### Test Categories

1. **Unit Tests** - Fast, isolated tests for business logic
   ```bash
   dotnet test test/BookLibrary.Domain.UnitTests
   dotnet test test/BookLibrary.Application.UnitTests
   ```

2. **Integration Tests** - Test application workflows with real database
   ```bash
   dotnet test test/BookLibrary.Application.IntegrationTests
   ```

3. **Functional Tests** - End-to-end API testing
   ```bash
   dotnet test test/BookLibrary.Api.FunctionTests
   ```

4. **Architecture Tests** - Enforce architectural rules
   ```bash
   dotnet test test/BookLibrary.ArchitectureTests
   ```

### Test Features
- **Testcontainers**: Real PostgreSQL and Redis for integration tests
- **WebApplicationFactory**: Full API testing environment
- **Arrange-Act-Assert**: Clear test structure
- **FluentAssertions**: Readable test assertions

## ??? Domain Model

### Core Entities

#### **User** ??
- Authentication and profile management
- Role-based permissions (Admin/User)
- Book borrowing and return history

#### **Library** ???
- Container for book collections
- Manages book inventory

#### **Book** ??
- Rich book information (title, author, country)
- Stock and availability tracking
- Status management (Available/Borrowed/Deleted)

#### **Loan** ??
- Book borrowing transactions
- Due date management
- Return tracking

#### **Review** ?
- User feedback on books
- Rating system (1-5 stars)
- Comment system

### Value Objects
- **Email**: Email validation and formatting
- **BookTitle**: Book title with business rules
- **Author**: Author information encapsulation
- **LoanPeriod**: Date range validation for loans

## ?? Configuration

### Environment Variables
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__Database=<postgresql-connection>
ConnectionStrings__Cache=<redis-connection>
JwtAuthentication__SecretKey=<your-secret-key>
```

### Key Configuration Files
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development environment
- `docker-compose.yml` - Container orchestration
- `launchSettings.json` - Development profiles

## ?? Monitoring & Observability

### Health Checks
- **Database**: PostgreSQL connectivity
- **Cache**: Redis connectivity
- **Endpoint**: `/health`

### Logging
- **Serilog**: Structured logging framework
- **Seq**: Centralized log aggregation
- **Request Logging**: HTTP request/response tracking
- **Error Tracking**: Global exception handling

### Monitoring Endpoints
```http
GET /health                    # Application health status
```

## ?? Security Features

### Authentication & Authorization
- **JWT Bearer Tokens**: Stateless authentication
- **Role-based Access**: Admin and User roles
- **Password Security**: BCrypt hashing
- **Token Expiration**: Configurable token lifetime

### API Security
- **HTTPS**: Secure communication
- **CORS**: Cross-origin request handling
- **Input Validation**: FluentValidation rules
- **Error Handling**: Secure error responses

## ?? Deployment

### Docker Deployment
```bash
# Build and deploy
docker-compose up -d

# View logs
docker-compose logs -f booklibrary.api

# Scale services
docker-compose up -d --scale booklibrary.api=3
```

### Production Considerations
- Use environment-specific configuration
- Implement proper secrets management
- Configure SSL certificates
- Set up monitoring and alerting
- Database backup and recovery
- Load balancing for multiple instances

## ?? Contributing

### Development Workflow
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Make your changes following the existing patterns
4. Add tests for new functionality
5. Ensure all tests pass: `dotnet test`
6. Run architecture tests: `dotnet test test/BookLibrary.ArchitectureTests`
7. Commit changes: `git commit -m 'Add amazing feature'`
8. Push to branch: `git push origin feature/amazing-feature`
9. Open a Pull Request

### Code Standards
- Follow Clean Architecture principles
- Implement proper error handling with Result pattern
- Add comprehensive unit and integration tests
- Use meaningful commit messages
- Document public APIs
- Follow C# coding conventions

### Testing Guidelines
- Write tests for all business logic
- Use AAA pattern (Arrange-Act-Assert)
- Mock external dependencies
- Test both success and failure scenarios
- Maintain high test coverage

## ?? Learning Resources

### Clean Architecture
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

### Domain-Driven Design
- [DDD Reference by Eric Evans](https://www.domainlanguage.com/ddd/reference/)
- [Implementing DDD by Vaughn Vernon](https://vaughnvernon.co/?page_id=168)

### CQRS & Event Sourcing
- [CQRS Journey by Microsoft](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/jj554200(v=pandp.10))
- [MediatR Documentation](https://github.com/jbogard/MediatR)

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Support

For questions and support:

1. **Issues**: Create a GitHub issue for bugs or feature requests
2. **Discussions**: Use GitHub Discussions for questions
3. **Documentation**: Check the inline code documentation
4. **Examples**: Review the test projects for usage examples

---

**Happy Coding! ??**

Built with ?? using Clean Architecture and .NET 8