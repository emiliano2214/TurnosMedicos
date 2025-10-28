using Microsoft.AspNetCore.Authorization;
using System;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    [Authorize(Policy = "Staff")]
    public class ConsultorioController : GenericController<Consultorio>
    {
        public ConsultorioController(ApplicationDbContext context) : base(context) { }
    }
}
