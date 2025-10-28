using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class Especialidad
    {
        [Key]
        public int IdEspecialidad { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public ICollection<Medico>? Medicos { get; set; }
    }
}
