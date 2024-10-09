using Metrics.Domain;
using Microsoft.EntityFrameworkCore;

namespace Metrics
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
        }
        public DbSet<Person> People { get; set; }
    }
}
