using FlickFrenzyBot_Web_App.Entities;
using Microsoft.EntityFrameworkCore;


namespace FlickFrenzyBot_Web_App.Database
{
    public class BotDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public BotDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<CurrentState> CurrentStates { get; set; }
    }
}
