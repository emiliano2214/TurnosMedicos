using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    public class TurnoController : GenericController<Turno>
    {
        public TurnoController(ApplicationDbContext context) : base(context) { }
    }
}
