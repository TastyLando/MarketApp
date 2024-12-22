# Market Project

An-commerce application developed using ASP.NET Core MVC.
## Features
- Product listing and detail views
 Shopping cart functionality
 Admin panel
 Product management
 MongoDB database integration
 Modern and responsive design
## Technologies Used
- ASP.NET Core MVC
 MongoDB
 Bootstrap 5
 jQuery
 Font Awesome
 Toastr.js
## Prerequisites
- .NET 7.0 SDK or later
 MongoDB
 Visual Studio 2022 or VS Code
## Installation
1. Clone the repository
   > git clone https://github.com/TastyLando/MarketApp.git

2. Navigate to the project directory
   > cd Market
3. Install MongoDB and start the service

4. Update the connection string in appsettings.json
   json
>{
"MongoDBSettings": {
"ConnectionString": "your_connection_string",
"DatabaseName": "MarketDB"
}
}
5. Run the application
>dotnet run

## Admin Access
To access the admin panel:
>- URL: `/Admin/Login`
>- Username: `admin`
>- Password: `123456`
