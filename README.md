# Rescue System Web API: REST API for Rescue System application
This is the backend API for the "Rescue System", a fault-tolerant monitoring application. It's designed to process emergency signals from users, even with incomplete or corrupted data. The core feature is a unique "Red/Yellow/Gray" alert classification system that ensures 100% of incoming signals are processed and categorized. The project is built on Clean Architecture principles to ensure modularity, testability, and scalability.

<img width="2444" height="1344" alt="tg_image_1352848666" src="https://github.com/user-attachments/assets/377e070f-3646-46ef-a5cc-8513c3e352cf" />

Desktop client:
[Rescue System Client](https://github.com/blugsam/Rescue-System-Client)

Domain model repository:
[Rescue System Class Library](https://github.com/blugsam/Rescue-System-Class-Library)

## Architecture overview
The application is built on the principles of **Clean Architecture**, with a clear separation of layers:

* **Domain:** Contains the application's domain entities.

* **Application:** Contains the application's business logic.

* **Infrastructure:** Contains the implementation details of the application, such as data access, external services, and other infrastructure components.

* **API:** Application entrance. (ASP.NET Core).

## Technologies Used
This project is built using the following technologies:

* **Platform:** .NET Core 9

* **Web-framework:** ASP.NET Core 9

* **ORM:** EntityFramework Core 9

* **Database:** PostgreSQL

* **Real-time:** SignalR

* **Logging:** Serilog

* **Validation:** FluentValidation

## Getting Started
To get started with this project, follow these steps:

1) Clone the repository using the following command:
    ```bash
    git clone https://github.com/blugsam/Rescue-System-Web-API
    ```

2) Create an appsettings.Development.json file in the main project directory. You can copy appsettings.example.json and fill in your PostgreSQL credentials

3) To setup postgres container run the following command in project root folder.
    ```bash
    docker-compose up
    ```

4) Run the following command for applying EntityFramework Core migrations.
    ```bash
    dotnet ef database update --project src/RescueSystem.Infrastructure --startup-project src/RescueSystem.Api
    ```

5) Make sure you have built the NuGet package as described in the library's README. Create a nuget.config file in the root folder of your project with the following content:

    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
      <packageSources>
        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
        <add key="LocalPackages" value="./local-packages" />
      </packageSources>
    </configuration>
    ```
    Create a local-packages folder nearby and copy the compiled RescueSystem.ClassLibrary.[VERSION].nupkg file into it.
    Run the command to add the package to the project:

    ```bash
    dotnet add package RescueSystem.ClassLibrary
    ```

5) Build and run the application
    ```bash
    dotnet run
    ```

6) The API is now available at https://localhost:5107. Navigate to /swagger to see the UI.
    ```bash
    http://localhost:5107/swagger/
    ```

## Endpoint overview

<img width="1134" height="879" alt="image" src="https://github.com/user-attachments/assets/3318f912-7d2b-43ef-b99b-9d8f05ef3a32" />
