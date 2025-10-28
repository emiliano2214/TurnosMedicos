using System.ComponentModel.DataAnnotations;

namespace TurnosMedicos.Models
{
    public class ObraSocial
    {
        [Key]
        public int IdObraSocial { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string? Plan { get; set; }

        [StringLength(50)]
        public string? Telefono { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        // Relación con Paciente
        public ICollection<Paciente>? Pacientes { get; set; }
    }
}
