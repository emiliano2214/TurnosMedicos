using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TurnosMedicos.Services;

namespace TurnosMedicos.Controllers
{
    [Route("api/ia")]
    [ApiController]
    [Authorize]
    public class IAApiController : ControllerBase
    {
        private readonly IIaChatService _chatService;

        public IAApiController(IIaChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Pregunta))
            {
                return BadRequest("La pregunta es requerida.");
            }

            // Determine role
            var rol = "Usuario";
            if (User.IsInRole("Admin")) rol = "Admin";
            else if (User.IsInRole("Medico")) rol = "Medico";
            else if (User.IsInRole("Paciente")) rol = "Paciente";
            else if (User.IsInRole("Administrativo")) rol = "Administrativo";

            try
            {
                var response = await _chatService.AskQuestionAsync(request.Pregunta, rol);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Return the error detail to the frontend for debugging
                return StatusCode(500, new { error = "Ocurrió un error en el servidor.", details = ex.Message });
            }
        }

        public class ChatRequest
        {
            public string Pregunta { get; set; }
        }
    }
}
