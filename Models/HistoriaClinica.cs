using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class HistoriaClinica
    {
        [Key]
        public int IdHistoria { get; set; }

        [Required]
        public int IdPaciente { get; set; }
        public Paciente? Paciente { get; set; }

        [Required]
        public int IdMedico { get; set; }
        public Medico? Medico { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required, StringLength(500)]
        public string Descripcion { get; set; }
    }
}
