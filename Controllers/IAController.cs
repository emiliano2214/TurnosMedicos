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
        private readonly IHttpClientFactory _httpClientFactory;

        public IAController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Pregunta))
            {
                return BadRequest("La pregunta no puede estar vacía.");
            }

            // Obtener rol del usuario
            var rol = "Usuario";
            if (User.IsInRole("Admin")) rol = "Admin";
            else if (User.IsInRole("Medico")) rol = "Medico";
            else if (User.IsInRole("Paciente")) rol = "Paciente";
            else if (User.IsInRole("Administrativo")) rol = "Administrativo";

            var payload = new
            {
                rol = rol,
                pregunta = request.Pregunta,
                historial = new List<string>() // Futuro: historial de chat
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            // Asumimos que el servicio corre en local, puerto 8000
            var response = await client.PostAsync("http://localhost:8000/chat", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return Ok(responseString);
            }

            return StatusCode((int)response.StatusCode, "Error al contactar al servicio de IA");
        }

        public class ChatRequest
        {
            public string Pregunta { get; set; }
        }
    }
}
