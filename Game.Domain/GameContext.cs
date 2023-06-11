using GameApp.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApp.Domain
{
    public class GameContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameProgress> GameProgresses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserGame> UserGames { get; set; }

        public GameContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGame>().HasKey(ug => new { ug.UserId, ug.GameId });
        }
    }
}
