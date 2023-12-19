using IdentityAppCourse2022.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityAppCourse2022.Data
{
    public class KivanoDbContext : DbContext
    {
        public KivanoDbContext(DbContextOptions<KivanoDbContext> options) : base(options)
        {

        }

        public DbSet<SearchProduct> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
