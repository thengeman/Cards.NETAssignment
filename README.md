# runnning the assigment

- Load the project in Visual Studio
- Right click cards project, select manage user secrets.
- Add SQL connection string to your database connection.
- run dotnet ef database update to run latest migration and seed users.
- The project has 3 users.
- Email member@test.com and anothermember@test.com have Member role. Default password is 'Member*123'
- Email admin@test.com has Admin role. Default password is 'Admin**123'.
- Start debugging application by running F5.
- This opens Swagger UI which you can use to check API documentation and start testing.
- Enjoy!