﻿using Badger.Data;
using Npgsql;

namespace WebApi.IntegrationTests.Data
{
    public static class DataUtils
    {
        public static ISessionFactory CreateSessionFactory()
        {
            return Badger.Data.SessionFactory.With(config =>
                config.WithConnectionString($"Host=localhost;Username=postgres;Password=password;Pooling=false;Database=content")
                      .WithProviderFactory(NpgsqlFactory.Instance));
        }
    }
}