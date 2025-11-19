using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosMedicos.Models
{
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }

        public string UserId { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required, StringLength(15)]
        public string Dni { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(15)]
        public string Telefono { get; set; }

        public DateTime FechaNacimiento { get; set; }

        [Required]
        public int IdObraSocial { get; set; }

        [ForeignKey(nameof(IdObraSocial))]
        public ObraSocial? ObraSocial { get; set; }

        // Relaciones
        public ICollection<Turno>? Turnos { get; set; }
        public ICollection<Tratamiento>? Tratamientos { get; set; }
        public ICollection<HistoriaClinica>? HistoriasClinicas { get; set; }
    }
}
