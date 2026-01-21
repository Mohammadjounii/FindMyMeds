# FindMyMeds

FindMyMeds is an ASP.NET Core web application designed to help users locate medication availability across registered pharmacies. The system allows pharmacies to manage their inventory, users to search for medications, and administrators to manage and approve pharmacies.

---

## Architecture
The project follows a **4-layer architecture** to ensure separation of concerns and maintainability:

- **Core**: Domain entities, DTOs, and interfaces  
- **Infrastructure**: Entity Framework Core DbContext, repositories, and database migrations  
- **Services**: Business logic, managers, and background services  
- **Web**: MVC controllers, API controllers, views, and SignalR hubs  

---

## User Roles
- **Admin**
  - Approves pharmacies
  - Views system reports  
- **Pharmacy**
  - Manages medication inventory
  - Receives low-stock notifications  
- **User**
  - Searches for medication availability
  - Views pharmacy details  

---

## Key Features
- Medication availability search  
- Pharmacy inventory management  
- Authentication and role-based authorization using ASP.NET Identity  
- Background service for low-stock notifications  
- Real-time updates using SignalR  
- Logging using Serilog  
- Unit testing for business logic  
- Responsive user interface using Bootstrap  

---

## API
The application exposes REST API endpoints that can be used by external systems to query medication availability. All available endpoints are documented using Swagger.

Swagger is available at:
/swagger

## Technologies Used
- ASP.NET Core  
- Entity Framework Core  
- SQL Server  
- ASP.NET Identity  
- Serilog  
- SignalR  
- Bootstrap  
- xUnit (Unit Testing)  

---

## How to Run the Project
1. Open the solution in **Visual Studio**  
2. Restore NuGet packages  
3. Update the connection string in `appsettings.json`  
4. Run database migrations  
5. Run the application  

---

## Background Services
A background hosted service periodically checks pharmacy inventory and automatically generates notifications when medication stock levels fall below a defined threshold.

---

## Responsive Design
The user interface is fully responsive and adapts to different screen sizes using Bootstrap’s grid system and responsive components.

---

## Project Presentation
A PowerPoint presentation explaining the system architecture, design decisions, and overall workflow is included as part of the project submission.
