# Havoc-API

**Havoc-API** is the backend service for the Havoc project, developed primarily in C#.  
This project was created as part of an Engineer Thesis at PJAIT (Polish-Japanese Academy of Information Technology).

## Table of Contents

- [About](#about)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## About

Havoc-API provides secure, scalable, and robust backend functionality for the Havoc ecosystem.  
It is designed to serve frontend applications (such as [havoc-spa](https://github.com/havoc-org/havoc-spa)) with RESTful endpoints and integrates modern backend best practices.
Our main goal was to create modular, easily scalable and manageble baseground for storing and supporting different parts of project management using mainly Microsoft Stack.

## Features

- 🚀 **RESTful API**: Provides endpoints for data access and business logic.
- 🛡️ **Authentication & Authorization**: Ready for integration with auth providers, token-based security.
- 📦 **Database Connectivity**: Easily integrates with relational - `SQL Server` - and/or NoSQL databases.
- 🧑‍💻 **Modular Architecture**: Clean separation of concerns for maintainability.
- 🧪 **Testing Setup**: Supports unit tests with 75% of code coverage.
- 🐳 **Docker Support**: Containerization for easy deployment.

## Tech Stack

- **C#** (97.3%)
- **Dockerfile** (1.1%)
- **SQL, Terraform and config files** (1.6%)

### Frameworks and libraries 
- ASP.NET Core
- Entity Framework
- Fluent Assertions
- xUnit
- Moq

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version matching project requirements)
- [Docker](https://www.docker.com/) (optional, for containerized runs)

### Installation

Clone the repository:

```bash
git clone https://github.com/havoc-org/havoc-api.git
cd havoc-api
```

Restore dependencies:

```bash
dotnet restore
```

### Running Locally

```bash
dotnet run
```

The API will be available at [http://localhost:5000](http://localhost:5000) (or as configured).

### Running with Docker

```bash
docker build -t havoc-api .
docker run -p 5000:5000 havoc-api
```

## Project Structure

```
havoc-api/
├── src/              # Source code
│   ├── Controllers/  # API controllers
│   ├── Models/       # Data models
│   ├── Services/     # Business logic/services
│   ├── Program.cs    # Main entry point
│   └── Startup.cs    # App configuration
├── tests/            # Test projects
├── Dockerfile        # Containerization setup
├── havoc-api.sln     # Solution file
└── README.md
```

## Contributing

You can fork this repo, however this one is going to be "archived" to preserve state of our work.

## License

This project is licensed under the [MIT License](LICENSE).

## Maintainers

- [Havoc Org](https://github.com/havoc-org)
- Created as part of Engineer Thesis at [Polish-Japanese Academy of Information Technology](https://pja.edu.pl/en/)

---

*Built with ❤️ as part of an Engineer Thesis at PJAIT.*
