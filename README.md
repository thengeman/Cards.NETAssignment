# Running the assigment

- Load the project in Visual Studio
- Right click cards project, select manage user secrets.
- Add SQL connection string to your database connection.
- Example connection string for SQLexpress 
 
*{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=**YourTestDB**;User Id=**YourTestUsername**;Password=**UserPassword**;Integrated Security=False;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}*

- Install Entity Framework Core tools by running ***dotnet tool install --global dotnet-ef***
- Run ***dotnet ef database update*** to run latest migration and seed users.
- The project will seed 3 users in the database.
    - Email ***member@test.com*** and ***anothermember@test.com**** have Member role. Default password is ***'Member*123'***
    - Email ***admin@test.com*** has Admin role. Default password is ***'Admin**123'***.
- Start debugging application by running ***Ctrl+F5***.
- This opens Swagger UI which you can use to check API documentation and start testing.
- Enjoy!