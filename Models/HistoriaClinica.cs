using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosMedicos.Models
{
    public class HistoriaClinica
    {
        [Key] public int IdHistoria { get; set; }

        [Required] public int IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))] public Paciente? Paciente { get; set; }

        [Required] public int IdMedico { get; set; }
        [ForeignKey(nameof(IdMedico))] public Medico? Medico { get; set; }

        // 🔗 Vinculación directa al turno atendido (nullable por compatibilidad)
        public int? IdTurno { get; set; }
        [ForeignKey(nameof(IdTurno))] public Turno? Turno { get; set; }

        [Required] public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Conservás tu campo original
        [StringLength(500)] public string? Descripcion { get; set; }

        // ✅ Campos que querés ver en el comprobante
        [StringLength(200)] public string? Diagnostico { get; set; }
        [StringLength(500)] public string? Tratamiento { get; set; }
    }
}
