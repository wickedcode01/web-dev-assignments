# Clinic Appointment System

A modern, containerized web application for managing clinic appointments built with ASP.NET Core MVC and SQLite.

## 🚀 Features

- Appointment scheduling and management
- User-friendly interface
- Containerized deployment with Docker
- Secure data storage with SQLite
- Built with modern ASP.NET Core MVC architecture

## 🏗️ Architecture

The application follows a clean MVC architecture:

- **Frontend**: ASP.NET MVC Views and Bootstrap (HTML/CSS/JS)
- **Backend**: ASP.NET MVC Controllers and Models
- **Data Layer**: Entity Framework Core with SQLite database

All components are containerized using Docker for easy deployment and scalability.

## 🛠️ Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQLite Database
- Docker
- HTML/CSS/JavaScript (wwwroot)

## 📋 Prerequisites

- .NET 8.0 SDK
- Docker Desktop
- Visual Studio 2022 or VS Code (recommended)

## 🚀 Getting Started

1. Clone the repository:
   ```bash
   git clone [repository-url]
   cd ClinicAppointment
   ```
2. Install Docker Desktop and Visual Studio  

2. Build and run with Docker:

   Open with Visual Studio:
   1. Open `ClinicAppointment.sln` in Visual Studio
   2. Press F5 or click the "Start Debugging" button to run the application.

3. Access the application at `https://localhost:8081` (Docker)

## 📁 Project Structure

```
ClinicAppointment/
├── Controllers/         # MVC Controllers
├── Models/             # Data models and business logic
├── Views/              # MVC Views
├── Services/           # Business services
├── Data/              # Data access layer
├── wwwroot/           # Static files (CSS, JS, images)
├── Dockerfile         # Docker configuration
└── Program.cs         # Application entry point
```

## 🔧 Wireframe
![Clinic Appointment System Wireframe](wireframe.jpeg)

## ER diagram

![ERD](ERD.png)

## 🐳 Docker Support

The application includes Docker support for containerized deployment:
- Multi-stage build for optimized container size
- Environment-specific configurations
- Ready for cloud deployment

