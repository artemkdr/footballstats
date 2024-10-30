using Testcontainers.PostgreSql;

namespace API.Tests
{
    public class TestContainers
    {
        private static PostgreSqlContainer? postgresContainer = null;

        private static object _lock = new();

        public static PostgreSqlContainer GetPostgresContainer(string? initSqlFile = "init.sql")
        {
            lock (_lock) {
                if (postgresContainer == null)
                {
                    var builder = new PostgreSqlBuilder().
                        WithImage("postgres:15-alpine");
                    if (File.Exists(initSqlFile)) {
                        builder = builder.WithResourceMapping(new FileInfo(initSqlFile), "/docker-entrypoint-initdb.d");
                    }
                    postgresContainer = builder.Build();
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
