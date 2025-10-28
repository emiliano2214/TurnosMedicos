using System;
using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Data;
using TurnosMedicos.Models;

namespace TurnosMedicos.Controllers
{
    public class EspecialidadController : GenericController<Especialidad>
    {
        public EspecialidadController(ApplicationDbContext context): base(context) { }
    
    }
}
