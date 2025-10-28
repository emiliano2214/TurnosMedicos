using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    public class PacienteController : GenericController<Paciente>
    {
        public PacienteController(ApplicationDbContext context) : base(context) { }
    }
}
