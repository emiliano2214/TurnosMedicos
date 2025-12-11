using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;

namespace TurnosMedicos.Controllers
{
    [Authorize]
    public class IAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
