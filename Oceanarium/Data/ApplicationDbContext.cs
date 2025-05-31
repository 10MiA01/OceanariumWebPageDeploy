using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Oceanarium.Identity;
using Oceanarium.Models;

namespace Oceanarium.Data
{
    public class ApplicationDbContext : IdentityDbContext<AdminUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base( options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Exibition> Exibition { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Oceanarium opening",
                    Description = "Join us for the grand opening of the Oceanarium.",
                    StartDate = new DateTime(2025, 5, 1, 10, 0, 0),
                    EndDate = new DateTime(2025, 5, 1, 18, 0, 0),
                    Price = 45.00m,
                    MaxTickets = 100
                }                
            );
            modelBuilder.Entity<Exibition>().HasData(
                new Exibition
                {
                    Id = 1,
                    Name = "Shark Aquarium",
                    Description = "Get close to the kings of the deep sea.",
                    IsPermanent = true,
                    ImageUrl = "/images/animals/shark.jpg"
                },
                new Exibition
                {
                    Id = 2,
                    Name = "Deep Sea Creatures",
                    Description = "Discover the mysteries of the ocean floor.",
                    IsPermanent = true,
                    ImageUrl = "/images/animals/octopus.jpg"
                },
                new Exibition
                {
                    Id = 3,
                    Name = "Jellyfish Room",
                    Description = "Step into the Jellyfish Room, where glowing creatures drift like living lanterns in a tranquil underwater ballet.",
                    IsPermanent = true,
                    ImageUrl = "/images/animals/jellyfish.jpg"
                },
                new Exibition
                {
                    Id = 4,
                    Name = "Lost Treasures of the Deep",
                    Description = "Unearth the secrets of sunken ships and forgotten civilizations. This exhibition showcases mysterious artifacts recovered from the ocean floor, each with a story lost to time.",
                    IsPermanent = false,
                    StartDate = new DateTime(2025, 5, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    ImageUrl = "/images/animals/ship.jpg"
                },
                new Exibition
                {
                    Id = 5,
                    Name = "Bioluminescent Creatures",
                    Description = "Discover nature’s own light show—an enchanting collection of glowing marine life that illuminates the darkest depths. From radiant jellyfish to twinkling deep-sea fish, witness the magic of bioluminescence.",
                    IsPermanent = false,
                    StartDate = new DateTime(2025, 1, 1),
                    EndDate = new DateTime(2025, 8, 1),
                    ImageUrl = "/images/animals/bioluminescent.jpg"
                }
            );
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 2,
                    Name = "Mermaid show",
                    Description = "See the beautiful underwater dance.",
                    StartDate = new DateTime(2025, 6, 1, 18, 0, 0),
                    EndDate = new DateTime(2025, 6, 1, 19, 0, 0),
                    Price = 45.00m,
                    MaxTickets = 30
                }
            );
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Tickets)
                .WithOne(o => o.Order)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
