using IdentityAppCourse2022.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityAppCourse2022.Data
{
    public class AsiaStoreDbContext : DbContext
    {
        public AsiaStoreDbContext(DbContextOptions<AsiaStoreDbContext> options) : base(options)
        {

        }

        public DbSet<SearchProduct> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
        }
    }
}
