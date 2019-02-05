using System;
using Dapper;
using Npgsql;

namespace DatabaseInitialiser
{
    public class Initialiser : IDisposable
    {
        private string Database { get; }
        private const string BaseConnectionString = "Host=localhost;Username=postgres;Password=password;Pooling=false";
        public string ConnectionString => $"{BaseConnectionString};Database={Database}";

        public Initialiser(string database)
        {
            Database = database;
        }

        public void Init()
        {
            CreateDatabase();
            CreateTable();
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

        private void CreateTable()
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Execute(@"create table events
                    (
                        id bigserial primary key,
                        eventId uuid not null,
                        partnerId uuid not null,
                        eventName varchar(100) not null,
                        addressLine1 varchar(100) not null,
                        postalCode varchar(10) not null,
                        city varchar(100) not null,
                        country varchar(100) not null,
                        latitude float8 not null,
                        longitude float8 not null
                    )");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating table: {ex.Message}");
            }
        }

        public void Dispose()
        {
            DestroyDatabase();
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
