using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace API.Tests
{
    public class TestContainers
    {
        private static PostgreSqlContainer? postgresContainer = null;

        private static object _lock = new();

        public static PostgreSqlContainer GetPostgresContainer()
        {
            lock (_lock) {
                if (postgresContainer == null)
                {
                    postgresContainer = new PostgreSqlBuilder().
                        WithImage("postgres:15-alpine").
                        WithResourceMapping(new FileInfo("init.sql"), "/docker-entrypoint-initdb.d").                        
                        Build();
                    postgresContainer.StartAsync().Wait(new TimeSpan(0, 1, 0));
                }
            }
            return postgresContainer;
        }

        public static string GetConnectionString()
        {
            return GetPostgresContainer().GetConnectionString();
        }
    }
}
