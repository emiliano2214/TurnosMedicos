using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "Staff")]
    public class ObraSocialController : GenericController<ObraSocial>
    {
        public ObraSocialController(ApplicationDbContext context) : base(context) { } 
    }
}
