using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosMedicos.Models
{
    public class Turno
    {
        [Key] 
        public int IdTurno { get; set; }

        [Required] public int IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public Paciente? Paciente { get; set; }

        [Required] public int IdMedico { get; set; }
        [ForeignKey(nameof(IdMedico))]
        public Medico? Medico { get; set; }

        [Required] public DateTime FechaHora { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } = "Pendiente";
    }
}
