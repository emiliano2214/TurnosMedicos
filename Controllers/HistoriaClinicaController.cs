using Microsoft.AspNetCore.Mvc;
using TurnosMedicos.Models;
using TurnosMedicos.Data;

namespace TurnosMedicos.Controllers
{
    public class HistoriaClinicaController : GenericController<HistoriaClinica>
    {
        public HistoriaClinicaController(ApplicationDbContext context): base(context) { }
    }
}
