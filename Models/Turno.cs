using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class Turno
    {
        [Key]
        public int IdTurno { get; set; }

        [Required]
        public int IdPaciente { get; set; }
        public Paciente? Paciente { get; set; }

        [Required]
        public int IdMedico { get; set; }
        public Medico? Medico { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, Cancelado
    }
}
