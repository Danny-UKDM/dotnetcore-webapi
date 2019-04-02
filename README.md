# .Net Core Web Api

[![Build status](https://ci.appveyor.com/api/projects/status/rlou2yhasuj4h1tl/branch/master?svg=true)](https://ci.appveyor.com/project/Danny-UKDM/dotnetcore-webapi/branch/master)

Just playing with a .Net Core 2.2 Web Api

## Getting Started

These instructions will get you a copy of the solution up and running on your local machine for development and testing purposes.

### Prerequisites

* [.NET Core SDK 2.2](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* [Docker](https://docs.docker.com/docker-for-windows/)

### Using The Solution

In a terminal window in the root of the solution;

* Restore dependencies by using the `dotnet restore` command.

* Build the solution by using the `dotnet build` command.

* To run all tests, including integration tests;

  1. Provision the LocalStack and PostgreSQL resources by using the `docker-compose up -d` command.
  2. Run all tests by using the `dotnet test` command.

* To run the Web API project locally;

  1. Provision the LocalStack and PostgreSQL resources by using the `docker-compose up -d` command.
  2. Run the application by using the `dotnet run --project WebApi/WebApi.csproj` command.

Running the Web API project locally will create a local test S3 Bucket, PostgreSQL database and table; before making the HTTP endpoints available.

On application close (`ctrl` + `c` from the terminal window), the test database will be dropped and the S3 bucket, destroyed.

### Swagger

This project uses [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to document the API - see `https://localhost:{port}/swagger/` when running the project.

## Built With

* [.Net Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/) - Framework
* [Docker](https://docs.docker.com/docker-for-windows/) - Infrastructure
* [LocalStack](https://github.com/localstack/localstack) - Local AWS Cloud Stack
* [Badger.Data](https://github.com/timbarker/Badger.Data) - Data Access
* [Xunit](https://xunit.github.io/) / [Fluent Assertions](https://fluentassertions.com/) - Test Framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

Huge hat tip to [Matt](https://github.com/mholland) and [Tim](https://github.com/timbarker) for making my life super easy with [Badger.Data](https://github.com/timbarker/Badger.Data), and for providing clean open-sourced code which inspired me to write some of my own.
