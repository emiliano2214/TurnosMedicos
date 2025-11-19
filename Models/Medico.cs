using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosMedicos.Models
{
    public class Medico
    {
        [Key]
        public int IdMedico { get; set; }

        public string UserId { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        public string Matricula { get; set; }

        [Required]
        [ForeignKey(nameof(Especialidad))]
        public int IdEspecialidad { get; set; }
        public Especialidad? Especialidad { get; set; }

        [Required]
        [ForeignKey(nameof(Consultorio))]
        public int IdConsultorio { get; set; }
        public Consultorio? Consultorio { get; set; }

        public ICollection<Turno>? Turnos { get; set; }
    }
}
