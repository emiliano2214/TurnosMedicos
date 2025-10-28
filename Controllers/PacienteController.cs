using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize(Policy = "Staff")]
    public class PacienteController : GenericController<Paciente>
    {
        public PacienteController(ApplicationDbContext context) : base(context) { }
    }
}
