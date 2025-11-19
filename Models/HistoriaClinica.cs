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

        public int? IdTurno { get; set; }
        [ForeignKey(nameof(IdTurno))] public Turno? Turno { get; set; }

        [Required] public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // ---- Datos clínicos ampliados (TODOS opcionales) ----
        [StringLength(200)] public string? MotivoConsulta { get; set; }
        [StringLength(500)] public string? Sintomas { get; set; }

        // Signos vitales
        public decimal? PresionSistolica { get; set; }   // mmHg
        public decimal? PresionDiastolica { get; set; }  // mmHg
        public decimal? FrecuenciaCardiaca { get; set; } // lpm
        public decimal? FrecuenciaRespiratoria { get; set; } // rpm
        public decimal? Temperatura { get; set; }        // °C
        public decimal? SaturacionO2 { get; set; }       // %

        // Antropometría
        public decimal? PesoKg { get; set; }
        public decimal? AlturaM { get; set; }
        public decimal? IMC { get; set; }                // se calcula si hay Peso/Altura

        // Antecedentes / alergias
        [StringLength(300)] public string? Antecedentes { get; set; }
        [StringLength(200)] public string? Alergias { get; set; }

        // Examen / estudios
        [StringLength(800)] public string? ExamenFisico { get; set; }
        [StringLength(400)] public string? EstudiosSolicitados { get; set; }

        // Diagnóstico / tratamiento (lo que ya usás en Comprobante)
        [StringLength(200)] public string? Diagnostico { get; set; }
        [StringLength(800)] public string? Tratamiento { get; set; }

        // Plan y observaciones
        [StringLength(800)] public string? Indicaciones { get; set; }
        public DateTime? ProximoControl { get; set; }
        [StringLength(800)] public string? Observaciones { get; set; }

        // Conservás tu campo original (opcional)
        [StringLength(500)] public string? Descripcion { get; set; }
    }
}
