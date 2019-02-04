# .Net Core Web Api

Just playing with a .Net Core 2.2 Web Api

## Getting Started

These instructions will get you a copy of the solution up and running on your local machine for development and testing purposes.

### Prerequisites

* [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* [PostgreSQL 9.5](https://www.postgresql.org/download/) (superuser with password: `password` & port: `5432`)

### Using The Solution

In a terminal window in the root of the solution;

* Restore dependencies by using the `dotnet restore` command.

* Build the solution by using the `dotnet build` command.

* Run all tests by using the `dotnet test` command.

* Run the Web API project locally by using the `dotnet run --project WebApi/WebApi.csproj` command.

Running the Web API project locally will create a local test PostgreSQL database, create a test table and seed it with test data; before making the HTTP endpoints available.

On application close (`ctrl` + `c` from the terminal window), the test database will be dropped and the connection disposed of.

## Built With

* [.Net Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/) - Framework
* [Badger.Data](https://github.com/timbarker/Badger.Data) - Data Access
* [Xunit](https://xunit.github.io/) / [Fluent Assertions](https://fluentassertions.com/) - Test Framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
