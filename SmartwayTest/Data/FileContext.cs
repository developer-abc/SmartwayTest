using Microsoft.EntityFrameworkCore;
using SmartwayTest.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SmartwayTest.Data
{
    public class FileContext : DbContext
    {
        public DbSet<FileModel> Files { get; set; }
        public DbSet<GroupModel> Groups { get; set; }

        public FileContext(DbContextOptions<FileContext> options) : base(options)
        {
            this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
