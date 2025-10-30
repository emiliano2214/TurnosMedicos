using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class Tratamiento
    {
        [Key]
        public int IdTratamiento { get; set; }

        [Required]
        public int IdPaciente { get; set; }
        public Paciente? Paciente { get; set; }

        [Required, StringLength(200)]
        public string? Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
