rem Smazat databázi
dotnet ef database drop --force --startup-project ../AVAIntegrationModeler.API
rem Smazat migrations   
dotnet ef migrations add InitialCreate --startup-project ../AVAIntegrationModeler.API