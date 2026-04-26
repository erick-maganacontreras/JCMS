Jewelry Cleaning Management System (JCMS)

The Jewelry Cleaning Management System (JCMS) is an ASP.NET Core Razor Pages web application designed to streamline the jewelry cleaning check-in process for store staff and administrators. The system helps track customers, jewelry items, cleaning orders, and staff access in one centralized application.

Live application: https://jcms-dev-web-e7h6e8e9fca3efe4.centralus-01.azurewebsites.net/ 

Features
•	Staff authentication with role-based access for Staff and Admin users.
•	Customer account creation, search, and updates.
•	Jewelry item management, including support for charm bracelet item structures.
•	Cleaning order creation, status tracking, and order history workflows.
•	Admin tools for staff account management and reporting.

System requirements
To run the project locally, the following software is required:
•	Windows 10 or Windows 11, or another operating system that supports the .NET SDK and SQL Server tooling.
•	.NET 8 SDK.
•	Visual Studio 2022 with ASP.NET and web development workload, or another IDE that supports ASP.NET Core.
•	SQL Server Management Studio 2022 for local development, or access to Azure SQL Database for cloud environments
•	Entity Framework Core CLI tools if migrations need to be applied from the command line.

Project structure
The solution uses a layered architecture to separate the web interface, business logic, and data access responsibilities. This improves maintainability and keeps the presentation layer from directly accessing the database layer.
Typical projects in the solution include:
•	JCMS.Web - ASP.NET Core Razor Pages web application and startup configuration.
•	JCMS.Application - business logic and application services.
•	JCMS.Infrastructure - Entity Framework Core data access, entities, repositories, and migrations. 

Setup and installation
1. Clone or download the solution
Clone the repository or download the project files to your local machine, then open the solution in Visual Studio 2022.

2. Restore NuGet packages
Restore project dependencies before building the application by running the command below.
dotnet restore

3. Verify the connection string
The local development configuration uses the JCMSDb connection string in appsettings.json. App Service and Azure SQL connection strings can override local settings in deployment environments without changing code.
Example local development connection string:
"ConnectionStrings": {
  "JCMSDb": "Server=(localdb)\\MSSQLLocalDB;Database=JCMS;Trusted_Connection=True;MultipleActiveResultSets=true"
}

4. Install EF Core tools if needed
If dotnet ef is not installed on your machine, install it globally by using the command below. This enables Entity Framework Core migration commands from the command line.
dotnet tool install --global dotnet-ef --version 8.*
 
5. Apply database migrations
The project includes an existing EF Core migration in JCMS.Infrastructure/Migrations. Apply the migration before running the application so required tables such as Staff are created. 
For local development use the command below:
dotnet ef database update --project JCMS.Infrastructure --startup-project JCMS.Web

6. Build the solution using the command below or from Visual Studio.
dotnet build

Running the application
Run from Visual Studio
1.	Open the solution in Visual Studio 2022.
2.	Set JCMS.Web as the startup project.
3.	Press F5 to run with debugging, or Ctrl+F5 to run without debugging.
4.	The application should open in your default browser.
Run from the command line
From the solution folder, run:
dotnet run --project JCMS.Web

The application will start using the local development configuration unless environment settings override it.

Default admin account
On startup, the application seeds a default admin account if one does not already exist in the database. 
Default development admin credentials:
•	Username: admin
•	Password: Admin123!

Azure deployment notes
The project is deployed to Azure App Service and can use Azure SQL Database through the JCMSDb connection string configured in App Service settings. App Service configuration values take precedence over appsettings.json, which allows local and cloud environments to use different connection strings safely. 
Hosted application URL:
•	https://jcms-dev-web-e7h6e8e9fca3efe4.centralus-01.azurewebsites.net/ 

Technology stack
•	Visual Studio 2022 for development.
•	ASP.NET Core Razor Pages [.NET 8]. 
•	C#. 
•	Entity Framework Core.
•	SQL Server Management Studio local development and Azure SQL Database for deployment.
•	Azure App Service for hosting.
