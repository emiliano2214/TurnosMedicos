using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    public class ObraSocialController : GenericController<ObraSocial>
    {
        public ObraSocialController(ApplicationDbContext context) : base(context) { } 
    }
}
