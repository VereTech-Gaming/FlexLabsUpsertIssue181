using ConsoleApp.BackgroundJobs;
using ConsoleApp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting up...");

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddDbContextPool<MyDbContext>(options => ConfigureDbContext(options, builder));
            builder.Services.AddHostedService<ReproduceStackoverflow>();

            using IHost host = builder.Build();
            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += delegate
            {
                cts.Cancel();
            };

            // auto apply EfCore Migrations
            await using (var scope = host.Services.CreateAsyncScope())
            {
                await using var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                Console.WriteLine("Applying Migrations...");

                await db.Database.MigrateAsync();
            }

            Console.WriteLine("Running...");

            // Run app until CTRL + C key combo is pressed
            await host.RunAsync(cts.Token);
        }

        private static void ConfigureDbContext(DbContextOptionsBuilder dbContextBuilder, IHostApplicationBuilder builder)
        {
            dbContextBuilder.UseNpgsql(GetDBConnectionString(builder.Configuration), sqlServerOptionsBuilder => sqlServerOptionsBuilder
                .MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
            );
        }

        private static string? GetDBConnectionString(IConfiguration configuration)
        {
            NpgsqlConnectionStringBuilder connectionStringBuilder = new()
            {
                Database = configuration.GetValue<string>("DATABASE_DBNAME", "flexlabs"),
                Pooling = true,
                MaxPoolSize = configuration.GetValue<int>("DATABASE_MAXPOOLSIZE", 200),
                MinPoolSize = configuration.GetValue<int>("DATABASE_MINPOOLSIZE", 20),
                Password = configuration.GetValue<string>("DATABASE_PASSWORD", "flexlabs"),
                Port = configuration.GetValue<int>("DATABASE_PORT", 5432),
                Host = configuration.GetValue<string>("DATABASE_HOST", "localhost"),
                Username = configuration.GetValue<string>("DATABASE_USERNAME", "flexlabs"),
                Timeout = 15,
                KeepAlive = 30,
                ApplicationName = nameof(Program),
                CommandTimeout = 600
            };
            string connString = connectionStringBuilder.ToString();
            return connString;
        }
    }
}
