using Microsoft.AspNetCore.Identity;
using Oceanarium.Data;
using Oceanarium.Identity;

namespace Oceanarium.Servises
{
    public class UsersCleanUpService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public UsersCleanUpService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        //For start-clean up
        public override  async Task StartAsync(CancellationToken cancellationToken)
        {
            await CleanUpTemporaryUsersAsync();
            await base.StartAsync(cancellationToken);
        }

        //Infinite cicle with certain frequency
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                await CleanUpTemporaryUsersAsync();
            }
        }

        //Main function
        private async Task CleanUpTemporaryUsersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var cutoffDate = DateTime.UtcNow.AddHours(-8); 

            var tempUsers = dbContext.Users
                .Where(u => u.IsTemporary && u.CreatedAt < cutoffDate)
                .ToList();

            foreach (var user in tempUsers)
            {
                await userManager.DeleteAsync(user);
            }
        }
    }
}
