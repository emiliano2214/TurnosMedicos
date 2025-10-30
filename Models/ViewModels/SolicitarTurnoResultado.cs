namespace TurnosMedicos.Models.ViewModels
{
    public class SolicitarTurnoResultado
    {
        public int IdTurno { get; set; }
        public string Paciente { get; set; } = "";
        public string Medico { get; set; } = "";
        public string Especialidad { get; set; } = "";
        public DateTime FechaEmisionTurno { get; set; }
        public DateTime FechaAtencion { get; set; }
        public string? Diagnostico { get; set; }
        public string? Tratamiento { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaHora { get; set; }
    }
}
