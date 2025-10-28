using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    public class TratamientoController : GenericController<Tratamiento>
    {
        public TratamientoController(ApplicationDbContext context) : base(context) { }
    }
}
