using System;
using System.Data.Common;
using Dapper;
using Npgsql;

namespace DatabaseInitialiser
{
    public class Initialiser : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public DbProviderFactory ProviderFactory { get; }
        private string Database { get; }
        private const string BaseConnectionString = "Host=localhost;Username=postgres;Password=password;Pooling=false";
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

        public Initialiser(string database)
        {
            Database = database;
            ProviderFactory = NpgsqlFactory.Instance;
        }

        public void Init()
        {
            CreateDatabase();
            OpenConnection();
            CreateTable();
            InsertTestData();
        }

        private void CreateDatabase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(BaseConnectionString))
                {
                    conn.Execute($"create database {Database}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database: {ex.Message}");
            }
        }

        private void OpenConnection()
        {
            try
            {
                Connection = ProviderFactory.CreateConnection();
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erroring opening connection: {ex.Message}");
            }
        }

        private void CreateTable()
        {
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
                        city varchar(100) not null,
                        country varchar(100) not null,
                        latitude float8 not null,
                        longitude float8 not null
                    )"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating table: {ex.Message}");
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

            try
            {
                Connection.Execute(insertSql, TestEvent1);
                Connection.Execute(insertSql, TestEvent2);
                Connection.Execute(insertSql, TestEvent3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting test data: {ex.Message}");
            }
        }

        public void Dispose()
        {
            DestroyDatabase();
            Connection.Close();
        }

        private void DestroyDatabase()
        {
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
                Console.WriteLine($"Error dropping database: {ex.Message}");
            }
        }
    }
}
