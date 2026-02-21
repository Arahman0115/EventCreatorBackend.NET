# Event Creator Backend API

A robust C# .NET backend service for managing events (protests/demonstrations) with secure JWT-based authentication, MySQL database, and geolocation features.

## Features

- **User Authentication**: JWT-based secure authentication with token validation
- **Event Management**: Full CRUD operations for creating, retrieving, updating, and deleting events
- **User Authorization**: Role-based access control ensuring users can only modify their own events
- **Geolocation Support**: Automatic geocoding to convert addresses to coordinates
- **Secure Token Management**: Token expiration, issuer validation, and audience verification
- **RESTful API**: Well-documented REST endpoints with Swagger/OpenAPI support
- **Cloud Integration**: AWS S3 support for file storage
- **CORS Support**: Configured for cross-origin requests from frontend applications

## Tech Stack

- **Framework**: .NET 8 (C#)
- **Database**: MySQL with Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **ORM**: Entity Framework Core with Pomelo MySQL provider
- **API Documentation**: Swagger/OpenAPI
- **Cloud**: AWS S3
- **Identity Management**: ASP.NET Core Identity

## Prerequisites

- .NET 8 SDK or later
- MySQL 8.0 or later
- AWS Account (optional, for S3 integration)
- Node.js (if running locally with npm)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Arahman0115/EventCreatorBackend.NET.git
cd EventCreatorBackend.NET/ProtestMapAPI
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Configure Database Connection

Update the `appsettings.json` file with your MySQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=ProtestMapDB;User=root;Password=yourpassword;"
  }
}
```

### 4. Configure JWT Settings

Add JWT configuration to `appsettings.json`:

```json
{
  "JwtSettings": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "ProtestMapAPI",
    "Audience": "ProtestMapAPIUsers",
    "TokenValidityMins": 60
  }
}
```

### 5. Configure AWS S3 (Optional)

Add AWS configuration to `appsettings.json`:

```json
{
  "AWS": {
    "Profile": "default",
    "Region": "us-east-1"
  }
}
```

### 6. Apply Database Migrations

```bash
dotnet ef database update
```

This will create all necessary tables based on the migrations.

## Running the Application

```bash
dotnet run
```

The API will be available at `http://localhost` (port 80)

**API Documentation** will be available at `http://localhost/swagger`

## API Endpoints

### Authentication

#### Register User
```
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe"
}

Response: 200 OK
{
  "message": "User registered successfully"
}
```

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}

Response: 200 OK
{
  "email": "user@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 60
}
```

### Events/Protests

#### Get All Events
```
GET /api/protest

Response: 200 OK
[
  {
    "id": 1,
    "title": "Climate Action Rally",
    "street": "123 Main St",
    "city": "San Francisco",
    "state": "CA",
    "zipCode": "94102",
    "date": "2025-03-15T10:00:00Z",
    "cause": "Climate Change",
    "latitude": 37.7749,
    "longitude": -122.4194,
    "createdByUserId": "user-id-123"
  }
]
```

#### Get Event by ID
```
GET /api/protest/{id}

Response: 200 OK
{
  "id": 1,
  "title": "Climate Action Rally",
  ...
}
```

#### Create Event (Requires Authentication)
```
POST /api/protest
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "title": "Climate Action Rally",
  "street": "123 Main St",
  "city": "San Francisco",
  "state": "CA",
  "zipCode": "94102",
  "date": "2025-03-15T10:00:00Z",
  "cause": "Climate Change"
}

Response: 201 Created
{
  "id": 1,
  "title": "Climate Action Rally",
  ...
  "latitude": 37.7749,
  "longitude": -122.4194
}
```

#### Get User's Events (Requires Authentication)
```
GET /api/protest/user-protests
Authorization: Bearer {accessToken}

Response: 200 OK
[
  {
    "id": 1,
    "title": "Climate Action Rally",
    ...
  }
]
```

#### Update Event (Requires Authentication, Creator Only)
```
PUT /api/protest/{id}
Authorization: Bearer {accessToken}
Content-Type: application/json

{
  "id": 1,
  "title": "Updated Event Title",
  ...
}

Response: 204 No Content
```

#### Delete Event (Requires Authentication, Creator Only)
```
DELETE /api/protest/{id}
Authorization: Bearer {accessToken}

Response: 204 No Content
```

#### Test Authorization
```
GET /api/protest/test-auth
Authorization: Bearer {accessToken}

Response: 200 OK
{
  "message": "Authorization successful",
  "userId": "user-id-123",
  "claims": [...]
}
```

## Authentication

The API uses JWT (JSON Web Tokens) for authentication:

1. **Register** - Create a new user account
2. **Login** - Get an access token using email and password
3. **Include Token** - Add the token to the `Authorization` header as: `Bearer {accessToken}`
4. **Token Validation** - The server validates:
   - Token signature
   - Token expiration
   - Issuer and audience claims

### JWT Token Claims

The issued JWT contains:
- `name`: User's email
- `nameid`: User's ID
- `exp`: Expiration time
- `iss`: Issuer (ProtestMapAPI)
- `aud`: Audience (ProtestMapAPIUsers)

## Database Schema

### Users Table
- `Id` (Primary Key)
- `UserName` (Email)
- `Email`
- `PasswordHash`
- `FullName`
- `Timestamp fields` (ConcurrencyStamp, etc.)

### Protests Table
- `Id` (Primary Key)
- `Title` (varchar 200)
- `Street` (varchar 255)
- `City` (varchar 100)
- `State` (varchar 50)
- `ZipCode` (varchar 20)
- `Date` (DateTime)
- `Cause` (varchar 500)
- `Latitude` (float)
- `Longitude` (float)
- `CreatedByUserId` (Foreign Key to Users)

## Geolocation

The API automatically geocodes event addresses to retrieve latitude and longitude coordinates using the Google Geocoding API. These coordinates are stored for map display and location-based queries.

## CORS Configuration

The API is configured to accept requests from:
- Frontend URL: `http://march-frontend.s3-website-us-east-1.amazonaws.com`

Modify the `Program.cs` CORS policy to include additional frontend URLs as needed.

## Deployment

### AWS Deployment

The application includes AWS S3 integration and is designed for AWS deployment:

1. Configure AWS credentials in `appsettings.json`
2. Ensure RDS MySQL database is set up
3. Deploy the application to:
   - AWS Elastic Beanstalk
   - AWS ECS
   - AWS App Runner
   - EC2 Instance

### Docker Deployment

The application runs on port 80 and is configured via Kestrel server. You can containerize it using Docker for deployment.

## Security Considerations

- **Password Hashing**: Passwords are hashed using ASP.NET Core Identity
- **JWT Secret**: Use a strong, minimum 32-character secret key
- **HTTPS**: Configure HTTPS in production (use certificate)
- **CORS**: Restrict CORS to trusted domains only
- **Authorization**: Creator-only modifications are enforced on Update/Delete operations

## Development

### Project Structure

```
ProtestMapAPI/
├── Controllers/          # API endpoints
├── Models/              # Data models
├── Services/            # Business logic (JWT, Geocoding)
├── Migrations/          # Database migrations
├── Filters/             # Custom filters
├── ApplicationDbContext.cs
└── Program.cs           # Configuration and startup
```

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Apply migrations:
```bash
dotnet ef database update
```

## Troubleshooting

### JWT Key Error
Ensure `JwtSettings:Key` is at least 32 characters long and configured in `appsettings.json`.

### Database Connection Error
Verify MySQL is running and the connection string in `appsettings.json` is correct.

### CORS Issues
Check that your frontend URL is added to the CORS policy in `Program.cs`.

### Geocoding Failures
Verify the Google Geocoding API is configured and has valid billing setup.

## License

This project is part of the EventCreator application suite.

## Contact

For questions or issues, please visit the [GitHub Repository](https://github.com/Arahman0115/EventCreatorBackend.NET).
