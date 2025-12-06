using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProyectoProgramacionDAL.Entidades;

namespace ProyectoProgramacionDAL.Contexto
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext() { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<BitacoraMovimiento> BitacoraMovimientos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=AppDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Solicitud>()
                .Property(p => p.MontoCredito)
                .HasColumnType("decimal(18,2)");
        }
    }
}