using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class Medico
    {
        [Key]
        public int IdMedico { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        public int Matricula { get; set; }

        [Required]
        public int IdEspecialidad { get; set; }
        public Especialidad? Especialidad { get; set; }

        [Required]
        public int IdConsultorio { get; set; }
        public Consultorio? Consultorio { get; set; }

        public ICollection<Turno>? Turnos { get; set; }
    }
}
