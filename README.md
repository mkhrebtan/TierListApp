# 🏆 TierListApp

A full-stack tier list creation and management application built with **ASP.NET Core Web API** and **Vanilla JavaScript**. This project demonstrates modern software architecture patterns, clean code principles, and comprehensive full-stack development skills.

## 📸 Screenshots

*Screenshots will be added here to showcase the application in action*

### Home Page
![Home Page](path/to/home-screenshot.png)

### Tier List Editor
![Tier List Editor](path/to/editor-screenshot.png)

### User Authentication
![Authentication](path/to/auth-screenshot.png)

## 🚀 Features

### Core Functionality
- **Interactive Tier List Creation**: Drag-and-drop interface for creating custom tier lists
- **Image Management**: Upload, organize, and manipulate images with cloud storage integration
- **Real-time Editing**: Live tier list editing functionality
- **User Authentication**: Secure JWT-based authentication and authorization
- **Responsive Design**: Works seamlessly across desktop and mobile devices

### Advanced Features
- **Cloud Storage Integration**: AWS S3 integration for scalable image storage
- **Real-time Data Synchronization**: Optimistic UI updates with server synchronization
- **Contextual Menus**: Right-click context menus for enhanced user experience
- **Color Customization**: Custom color picker for tier ranking customization

## 🏗️ Architecture & Design Patterns

### Backend Architecture (ASP.NET Core)

#### **Clean Architecture Implementation**
```
📁 TierList.Domain/          # Core business logic
├── Entities/                # Domain entities
├── ValueObjects/            # Value objects
├── Repos/                   # Repository interfaces
├── Abstraction/             # Base classes and interfaces
└── Shared/                  # Shared domain logic

📁 TierList.Application/     # Application layer
├── Commands/                # CQRS Command handlers
├── Queries/                 # CQRS Query handlers
├── Common/                  # DTOs, interfaces, services
└── DependencyInjection.cs  # Service registration

📁 TierList.Infrastructure/  # External concerns
├── Services/                # External service implementations
└── Settings/                # Configuration settings

📁 TierList.Persistence/     # Data access layer
├── Configurations/          # Entity Framework configurations
├── Migrations/              # Database migrations
├── Repos/                   # Repository implementations
└── TierListDbContext.cs     # Database context

📁 TierListAPI/              # Presentation layer
├── Endpoints/               # Minimal API endpoints
├── Helpers/                 # API helpers and utilities
└── Program.cs               # Application entry point
```

#### **Implemented Patterns & Principles**

##### 🎯 **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Handle state-changing operations (Create, Update, Delete)
- **Queries**: Handle data retrieval operations
- **Handlers**: Separate handlers for each command and query
- **Benefits**: Clear separation of concerns, optimized read/write operations

##### 🏛️ **Repository Pattern**
- Generic repository base class for common operations
- Specific repositories for complex domain-specific queries
- Database abstraction for testability
- Interface-based design for easy testing and mocking

##### 🏢 **Unit of Work Pattern**
- Centralized transaction management across repositories
- Ensures data consistency across multiple operations
- Atomic commits for complex business operations
- Clean separation of business logic from data persistence
- Rollback capabilities for failed operations

##### 🔧 **Dependency Injection**
- Constructor injection throughout the application
- Service lifetime management (Scoped, Singleton, Transient)
- Clean separation of dependencies
- Testable and maintainable code structure

##### 📊 **Domain-Driven Design (DDD)**
- Rich domain models with business logic encapsulation
- Value objects for immutable data
- Aggregate roots for consistency boundaries
- Domain events for cross-cutting concerns

##### 🛡️ **Result Pattern**
- Functional error handling without exceptions
- Type-safe operation results
- Clear success/failure indication
- Detailed error information

### Frontend Architecture (Vanilla JavaScript)

#### **Component-Based Architecture**
- **Modular Design**: Separate modules for different concerns
- **View Components**: Dedicated view files for each page/feature
- **Data Management**: Centralized data management with caching
- **Event-Driven**: Event-driven communication between components

#### **Implemented Patterns**

##### 📦 **Module Pattern**
- ES6 modules for code organization
- Clear import/export structure
- Namespace management
- Dependency management

##### 🔄 **Observer Pattern**
- Event-driven architecture
- Loose coupling between components
- Reactive UI updates
- State change notifications

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens with refresh token support
- **Cloud Storage**: AWS S3 for image storage
- **Validation**: FluentValidation
- **API Documentation**: OpenAPI/Swagger with Scalar UI
- **Containerization**: Docker & Docker Compose

### Frontend
- **Core**: Vanilla JavaScript (ES6+)
- **Styling**: Custom CSS with responsive design
- **Drag & Drop**: SortableJS library
- **Color Picker**: Pickr color picker library
- **Icons**: Font Awesome
- **Build Tools**: Native ES6 modules

### Development & Deployment
- **Version Control**: Git
- **IDE**: Visual Studio / VS Code
- **Database Migrations**: Entity Framework Core Migrations
- **Environment Management**: Docker Compose with environment files
- **API Testing**: Built-in HTTP client files

## 🔒 Security Features

### Authentication & Authorization
- **JWT Token Authentication**: Secure token-based authentication
- **Refresh Token System**: Automatic token renewal
- **Protected Routes**: Route-level authorization
- **Password Hashing**: BCrypt password hashing

### Data Security
- **Input Validation**: Comprehensive input validation with FluentValidation
- **SQL Injection Prevention**: Entity Framework Core parameterized queries
- **XSS Protection**: Proper data encoding and validation
- **Secure Headers**: Security headers implementation

## 📊 Database Design

### Entity Relationships
- **Users**: User accounts with authentication data
- **TierLists**: User-owned tier list collections
- **TierRows**: Individual tier rankings within lists
- **TierImages**: Images associated with tier items
- **RefreshTokens**: Secure token management

### Data Integrity
- **Foreign Key Constraints**: Referential integrity enforcement
- **Cascade Deletes**: Proper cleanup of related data
- **Indexes**: Optimized query performance
- **Migrations**: Version-controlled schema changes

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK**
- **Docker & Docker Compose**
- **PostgreSQL** (or use Docker)
- **AWS Account** (for S3 storage)

### Environment Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/TierListApp.git
   cd TierListApp
   ```

2. **Configure environment variables**
   Create a `.env` file in the root directory:
   ```env
   # Database
   POSTGRES_DB=tierlistdb
   POSTGRES_USER=your_username
   POSTGRES_PASSWORD=your_password
   
   # JWT
   JWT_SECRET_KEY=your_base64_encoded_secret_key
   JWT_ISSUER=TierListApp
   JWT_AUDIENCE=TierListApp
   
   # AWS S3
   AWS_ACCESS_KEY_ID=your_access_key
   AWS_SECRET_ACCESS_KEY=your_secret_key
   AWS_REGION=your_region
   S3_BUCKET_NAME=your_bucket_name
   ```

3. **Start the application**
   ```bash
   docker-compose up -d
   ```

4. **Access the application**
   - **API**: https://localhost:5001
   - **Frontend**: http://localhost:5500 (or your preferred local server)
   - **API Documentation**: https://localhost:5001/scalar/v1

## 📋 API Endpoints

### Authentication
- `POST /auth/register` - User registration
- `POST /auth/login` - User login
- `POST /auth/refresh` - Token refresh

### Tier Lists
- `GET /lists` - Get user's tier lists
- `GET /lists/{id}` - Get specific tier list
- `POST /lists` - Create new tier list
- `PUT /lists/{id}` - Update tier list
- `DELETE /lists/{id}` - Delete tier list

### Tier Rows
- `POST /rows` - Create tier row
- `PUT /rows/{id}/rank` - Update row rank
- `PUT /rows/{id}/order` - Update row order
- `DELETE /rows/{id}` - Delete tier row

### Images
- `GET /images/upload-url` - Get S3 upload URL
- `POST /images` - Save image metadata
- `PUT /images/{id}/move` - Move image between tiers
- `PUT /images/{id}/reorder` - Reorder images within tier
- `DELETE /images/{id}` - Delete image

## 🔄 Development Workflow

### Code Quality
- **Clean Code Principles**: SOLID principles implementation
- **Code Documentation**: Comprehensive XML documentation
- **Error Handling**: Robust error handling throughout

### Version Control
- **Git Flow**: Feature-based development workflow
- **Commit Standards**: Descriptive commit messages
- **Code Reviews**: Pull request review process

## 🎯 Key Achievements

### Technical Excellence
- ✅ **Clean Architecture**: Proper separation of concerns
- ✅ **CQRS Implementation**: Command and query separation
- ✅ **Domain-Driven Design**: Rich domain models
- ✅ **Cloud Integration**: AWS S3 storage integration
- ✅ **Security Best Practices**: JWT authentication, input validation

### User Experience
- ✅ **Intuitive Interface**: Drag-and-drop functionality
- ✅ **Error Handling**: User-friendly error messages
- ✅ **Cross-platform**: Works on all modern browsers

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

*This project demonstrates advanced full-stack development capabilities, modern architectural patterns, and best practices in software engineering.*