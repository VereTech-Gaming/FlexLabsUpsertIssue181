internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        IResourceBuilder<ParameterResource> username = builder.AddParameter("username", builder.Configuration["DATABASE_USERNAME"] ?? "flexlabs", false, true);
        IResourceBuilder<ParameterResource> password = builder.AddParameter("password", builder.Configuration["DATABASE_PASSWORD"] ?? "flexlabs", false, true);

        IResourceBuilder<PostgresServerResource> postgres = builder.AddPostgres(builder.Configuration["DATABASE_HOST"] ?? "localhost", username, password)
            //.WithEnvironment("POSTGRES_USER", builder.Configuration["DATABASE_USERNAME"] ?? "flexlabs")
            //.WithEnvironment("POSTGRES_PASSWORD", builder.Configuration["DATABASE_PASSWORD"] ?? "flexlabs")
            .WithHostPort(int.TryParse(builder.Configuration["DATABASE_PORT"], out int port) ? port : 5432)
            .WithImageTag("17-alpine");

        IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres.AddDatabase(builder.Configuration["DATABASE_DBNAME"] ?? "flexlabs");

        builder.AddProject<Projects.ConsoleApp>("consoleapp")
            .WithReference(postgresdb)
            .WaitFor(postgresdb)
            .WithEnvironment("DATABASE_HOST", builder.Configuration["DATABASE_HOST"] ?? "localhost")
            .WithEnvironment("POSTGRES_USER", builder.Configuration["DATABASE_USERNAME"] ?? "flexlabs")
            .WithEnvironment("POSTGRES_PASSWORD", builder.Configuration["DATABASE_PASSWORD"] ?? "flexlabs")
            .WithEnvironment("DATABASE_PORT", builder.Configuration["DATABASE_PORT"] ?? "5432");


        builder.Build().Run();
    }
}