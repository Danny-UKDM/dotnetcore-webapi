using System;
using System.Data.Common;
using Badger.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using WebApi.Models;

namespace WebApi.Tools
{
    public class DbFactory : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public DbProviderFactory ProviderFactory { get; }
        private string Database { get; }

        private readonly ILogger _logger = ApplicationLogging.CreateLogger<DbFactory>();

        private const string BaseConnectionString = "Host=localhost;Username=postgres;Password=password;Pooling=false;Port=5433";
        public string ConnectionString => $"{BaseConnectionString};Database={Database}";

        private readonly Event TestEvent1 = new Event
        {
            EventId = 1001,
            PartnerId = Guid.NewGuid(),
            EventName = "A Most Excellent Event",
            AddressLine1 = "42 Smashing Avenue",
            PostalCode = "L0L H41",
            City = "Fabulousville",
            Country = "United Kingdom",
            Latitude = new Random().Next(-90, 90),
            Longitude = new Random().Next(-180, 180)
        };

        private readonly Event TestEvent2 = new Event
        {
            EventId = 2002,
            PartnerId = Guid.NewGuid(),
            EventName = "The One That The Guy From Skins Goes To",
            AddressLine1 = "123 Basic Way",
            PostalCode = "NOP3 T44",
            City = "Whocares",
            Country = "Pooperama",
            Latitude = new Random().Next(-90, 90),
            Longitude = new Random().Next(-180, 180)
        };

        private readonly Event TestEvent3 = new Event
        {
            EventId = 3003,
            PartnerId = Guid.NewGuid(),
            EventName = "This One's Alright",
            AddressLine1 = "8008 Hat Street",
            PostalCode = "432478",
            City = "Okayshire",
            Country = "Commonwealth of Sure",
            Latitude = new Random().Next(-90, 90),
            Longitude = new Random().Next(-180, 180)
        };

        public DbFactory(string database)
        {
            Database = database;
            ProviderFactory = NpgsqlFactory.Instance;
        }

        public void InitDatabase()
        {
            CreateDatabase();
            OpenConnection();
            CreateTable();
            InsertTestData();
        }

        private void CreateDatabase()
        {
            _logger.LogInformation($"Creating database: {Database}");
            try
            {
                using (var conn = new NpgsqlConnection(BaseConnectionString))
                {
                    conn.Execute($"create database {Database}");
                }
                _logger.LogInformation($"Successfully created database: {Database}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating database: {Database}", ex);
            }
        }

        private void OpenConnection()
        {
            _logger.LogInformation("Opening connection.");
            try
            {
                Connection = ProviderFactory.CreateConnection();
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            catch (Exception ex)
            {
                _logger.LogError("Erroring opening connection.", ex);
            }
        }

        private void CreateTable()
        {
            _logger.LogInformation("Creating table.");
            try
            {
                Connection.Execute(
                    @"create table events
                    (
                        id bigserial primary key,
                        eventId int not null,
                        partnerId uuid not null,
                        eventName varchar(100) not null,
                        addressLine1 varchar(100) not null,
                        postalCode varchar(10) not null,
                        city varchar(20) not null,
                        country varchar(20) not null,
                        latitude float8 not null,
                        longitude float8 not null
                    )"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating table.", ex);
            }
        }

        private void InsertTestData()
        {
            const string insertSql = @"
                    insert into events (
                        eventId,
                        partnerId,
                        eventName,
                        addressLine1,
                        postalCode,
                        city,
                        country,
                        latitude,
                        longitude
                        )
                        values
                        (
                        @EventId,
                        @PartnerId,
                        @EventName,
                        @AddressLine1,
                        @PostalCode,
                        @City,
                        @Country,
                        @Latitude,
                        @Longitude
                    )";

            _logger.LogInformation("Inserting test data.");
            try
            {
                Connection.Execute(insertSql, TestEvent1);
                Connection.Execute(insertSql, TestEvent2);
                Connection.Execute(insertSql, TestEvent3);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting test data", ex);
            }
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString(ConnectionString)
                      .WithProviderFactory(ProviderFactory));
        }

        public void Dispose()
        {
            DestroyDatabase();
            Connection.Close();
        }

        private void DestroyDatabase()
        {
            _logger.LogInformation($"Dropping database: {Database}");
            try
            {
                using (var conn = new NpgsqlConnection(BaseConnectionString))
                {
                    conn.Execute($@"
                        select pg_terminate_backend(pg_stat_activity.pid)
                        from pg_stat_activity
                        where pg_stat_activity.datname = '{Database}'");

                    conn.Execute($"drop database {Database}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error dropping database: {ex.Message}");
            }
        }
    }
}
