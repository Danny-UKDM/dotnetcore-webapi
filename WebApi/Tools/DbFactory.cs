using System;
using System.Data.Common;
using Badger.Data;
using Dapper;
using Npgsql;
using WebApi.Models;

namespace WebApi.Tools
{
    internal class DbFactory : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public DbProviderFactory ProviderFactory { get; }
        private string Database { get; }

        private readonly string _baseConnectionString = "Host=localhost;Username=postgres;Password=password;Pooling=false";
        public string ConnectionString => $"{_baseConnectionString};Database={Database}";

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

        public DbFactory(DbProviderFactory providerFactory)
        {
            Database = "content";
            ProviderFactory = providerFactory;

            InitDatabase();
        }

        private void InitDatabase()
        {
            CreateDatabase();
            OpenConnection();
            CreateTable();
            InsertTestData();
        }

        private void CreateDatabase()
        {
            using (var conn = new NpgsqlConnection(_baseConnectionString))
            {
                conn.Execute($"create database {Database}");
            }
        }

        private void OpenConnection()
        {
            Connection = ProviderFactory.CreateConnection();
            Connection.ConnectionString = ConnectionString;
            Connection.Open();
        }

        private void CreateTable()
        {
            Connection.Execute(
                @"create table events(
                    id bigserial primary key,
                    eventId int not null,
                    partnerId uuid not null,
                    eventName varchar(100) not null, 
                    addressLine1 varchar(100) not null
                    postalCode varchar(10) not null
                    city varchar(20) not null
                    country varchar(20) not null
                    latitude double not null
                    longitude double not null)"
            );
        }

        private void InsertTestData()
        {
            var insertSql = @"
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
                        @Country
                        @Latitude
                        @Longitude)";

            Connection.Execute(insertSql, TestEvent1);
            Connection.Execute(insertSql, TestEvent2);
            Connection.Execute(insertSql, TestEvent3);
        }

        public ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.With(config =>
                config.WithConnectionString(ConnectionString)
                      .WithProviderFactory(ProviderFactory));
        }

        public void Dispose()
        {
            Connection.Dispose();
            DestroyTestDatabase();
        }

        private void DestroyTestDatabase()
        {
            using (var conn = new NpgsqlConnection(_baseConnectionString))
            {
                conn.Execute($"drop database {Database}");
            }
        }
    }
}
