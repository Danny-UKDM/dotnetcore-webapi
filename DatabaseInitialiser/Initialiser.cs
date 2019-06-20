using System;
using Dapper;
using Npgsql;

namespace DatabaseInitialiser
{
    public class Initialiser
    {
        private readonly string _connectionString;
        private readonly string _adminConnectionString;
        private readonly string _database;

        public Initialiser(string connectionString)
        {
            _connectionString = connectionString;
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            _database = builder.Database;
            builder.Database = builder.Username;
            _adminConnectionString = builder.ToString();
        }

        public void Init()
        {
            CreateDatabase();
            CreateTables();
        }

        private void CreateDatabase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_adminConnectionString))
                {
                    conn.Execute($"create database {_database}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating database: {ex.Message}");
            }
        }

        private void CreateTables()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Execute(@"
create table events
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
    longitude float8 not null,
    createdAt timestamp not null,
    occursOn timestamp not null
);

create table videos
(
    id bigserial primary key,
    videoId uuid not null,
    partnerId uuid not null,
    videoName varchar(100) not null,
    url varchar(50) not null
);");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
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
                using (var conn = new NpgsqlConnection(_adminConnectionString))
                {
                    conn.Execute($@"
                        select pg_terminate_backend(pg_stat_activity.pid)
                        from pg_stat_activity
                        where pg_stat_activity.datname = '{_database}'");

                    conn.Execute($"drop database {_database}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error dropping database: {ex.Message}");
            }
        }
    }
}
