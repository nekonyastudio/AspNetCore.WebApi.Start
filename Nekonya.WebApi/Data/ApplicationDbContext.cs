using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Nekonya.Model.Entities;

namespace Nekonya.WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable("students");  
        }
    }
}
