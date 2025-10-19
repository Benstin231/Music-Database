using Microsoft.EntityFrameworkCore;
using s1121735_Final_Project.Models;

namespace s1121735_Final_Project.Data
{
    public class CmsContext : DbContext
    {
        public CmsContext(DbContextOptions<CmsContext> options) : base(options)
        {
        }

        public DbSet<Songs> TableMusicDB1121735 { get; set; }
        public DbSet<User> TableUserDB1121735 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
