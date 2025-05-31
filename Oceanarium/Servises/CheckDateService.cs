using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Oceanarium.Identity;
using Oceanarium.Models;

namespace Oceanarium.Servises
{
    public class CheckDateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CheckDateService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        //For start
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await CkeckDates();
            await base.StartAsync(cancellationToken);
        }

        //Infinite cicle with certain frequency
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                await CkeckDates();
            }
        }

        //Main function
        private async Task CkeckDates()
        {

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var events = await db.Events
                .Where(e => e.EndDate < DateTime.UtcNow)
                .Include(e => e.Tickets)
                .ThenInclude(t => t.Order)
                .ToListAsync();

            var exibitions = await db.Exibition
                .Where(e => e.IsPermanent == false && e.EndDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var e in events)
            {
                e.Status = "Finished";

                foreach (var ticket in e.Tickets)
                {
                    ticket.Status = "Finished";

                    if (ticket.Order != null)
                    {
                        ticket.Order.OrderStatus = "Finished";
                    }
                }
            }
            await db.SaveChangesAsync();

        }
    }
}
