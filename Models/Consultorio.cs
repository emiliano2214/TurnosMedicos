using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class Consultorio
    {
        [Key]
        public int IdConsultorio { get; set; }

        [Required, StringLength(50)]
        public string Numero { get; set; }

        [StringLength(100)]
        public string? Ubicacion { get; set; }

        public ICollection<Medico>? Medicos { get; set; }
    }
}
