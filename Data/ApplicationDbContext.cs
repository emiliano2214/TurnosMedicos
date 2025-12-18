using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Migrations;
using TurnosMedicos.Models;
using TurnosMedicos.Models.ViewModels;

namespace TurnosMedicos.Data
{
    public class ApplicationDbContext : IdentityDbContext<UsuarioExt>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<Medico> Medico { get; set; }
        public DbSet<Especialidad> Especialidad { get; set; }
        public DbSet<Turno> Turno { get; set; }
        public DbSet<HistoriaClinica> HistoriaClinica { get; set; }
        public DbSet<Tratamiento> Tratamiento { get; set; }
        public DbSet<Consultorio> Consultorio { get; set; }
        public DbSet<ObraSocial> ObraSocial { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 👇 Mapeo del resultado del procedimiento almacenado
            modelBuilder.Entity<SolicitarTurnoResultado>()
                .HasNoKey()       // No tiene clave primaria
                .ToView(null);    // No corresponde a una vista ni tabla
        }
    }
}
