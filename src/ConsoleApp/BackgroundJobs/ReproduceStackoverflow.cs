using ConsoleApp.Domain.Entities;
using ConsoleApp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.BackgroundJobs
{
    public class ReproduceStackoverflow : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public ReproduceStackoverflow(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        private ulong GetUlong()
        {
            return (ulong)Random.Shared.NextInt64(0, long.MaxValue);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Beginning crash loop...");

            // incase it doesnt break the first time, 
            // continually reproduce the environment where bug is exposed.
            while (!stoppingToken.IsCancellationRequested) 
            {
                // resolve dependencies
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                await using var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                try
                {
                    // reproduce error
                    await db.Set<DiscordGuild>()
                    .Upsert(new DiscordGuild()
                    {
                        Id = GetUlong()
                    })
                    .On(on => on.Id)
                    .RunAsync(stoppingToken);
                }
                catch (System.StackOverflowException ex) 
                {
                    // can an overflow exception even be caught?
                    Console.WriteLine("oh no not again, said the bowl of petunias.");
                }
            }
        }
    }
}
