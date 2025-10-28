namespace TurnosMedicos.Models.ViewModels
{
    public class ResumenSistemaViewModel
    {
        public string Usuario { get; set; }
        public int TotalPacientes { get; set; }
        public int TotalMedicos { get; set; }
        public int TotalTurnos { get; set; }
        public int TurnosPendientes { get; set; }
        public int TurnosConfirmados { get; set; }
        public int TotalEspecialidades { get; set; }
        public int TotalObrasSociales { get; set; }
        public int TotalConsultorios { get; set; }

        public List<Paciente>? Pacientes { get; set; }
        public List<Medico>? Medicos { get; set; }
        public List<Turno>? Turnos { get; set; }
    }
}
