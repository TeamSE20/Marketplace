using IdentityAppCourse2022.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityAppCourse2022.Data
{
    public class SoftTechDbContext : DbContext
    {
        public SoftTechDbContext(DbContextOptions<SoftTechDbContext> options) : base(options)
        {

        }

        public DbSet<SearchProduct> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
