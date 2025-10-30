namespace TurnosMedicos.Models.ViewModels
{
    public class TurnoForm
    {
        // Turno
        public int IdTurno { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "Pendiente";

        // Historia clínica (opcionales)
        public string? Diagnostico { get; set; }
        public string? Tratamiento { get; set; }
    }
}
