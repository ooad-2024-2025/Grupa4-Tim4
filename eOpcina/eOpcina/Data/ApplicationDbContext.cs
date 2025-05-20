using eOpcina.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eOpcina.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Korisnik> Korisnik { get; set; }
        public DbSet<Zahtjev> Zahtjev { get; set; }
        public DbSet<Dokument> Dokument { get; set; }
        public DbSet<Sablon> Sablon { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("Korisnik");
            modelBuilder.Entity<Zahtjev>().ToTable("Zahtjev");
            modelBuilder.Entity<Dokument>().ToTable("Dokument");
            modelBuilder.Entity<Sablon>().ToTable("Sablon");
            base.OnModelCreating(modelBuilder);
        }
    }
}
