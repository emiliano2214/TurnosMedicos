using System;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    public class ConsultorioController : GenericController<Consultorio>
    {
        public ConsultorioController(ApplicationDbContext context) : base(context) { }
    }
}
