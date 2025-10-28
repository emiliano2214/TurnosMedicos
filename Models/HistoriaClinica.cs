using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosMedicos.Models
{
    public class HistoriaClinica
    {
        [Key] public int IdHistoria { get; set; }

        [Required] public int IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public Paciente? Paciente { get; set; }

        [Required] public int IdMedico { get; set; }
        [ForeignKey(nameof(IdMedico))]
        public Medico? Medico { get; set; }

        [Required] public DateTime FechaRegistro { get; set; }

        [Required, StringLength(500)]
        public string Descripcion { get; set; } = "";
    }
}
