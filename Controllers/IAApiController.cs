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
                var iaResponse = await _chatService.AskQuestionAsync(request.Pregunta, rol);
                
                // Map to DTO
                var dto = new TurnosMedicos.Models.Dto.ChatIaResponseDto
                {
                    RespuestaUsuario = iaResponse.Answer,
                    SqlSugerido = iaResponse.SuggestedSql,
                    
                    // Populate Debug Info
                    Debug = new TurnosMedicos.Models.Dto.DebugDto
                    {
                        Documentos = !string.IsNullOrWhiteSpace(iaResponse.Notes) 
                            ? new System.Collections.Generic.List<string> { iaResponse.Notes } 
                            : new System.Collections.Generic.List<string>(),
                        ReglasAplicadas = iaResponse.RulesApplied,
                        Confianza = iaResponse.ConfidenceScore,
                        PromptFinal = "N/A (Local Rules)", // or construct if relevant
                        MsRetrieval = iaResponse.RetrievalTimeMs,
                        MsGeneracion = iaResponse.GenerationTimeMs
                    }
                };

                // Filter for non-Admins
                if (!User.IsInRole("Admin"))
                {
                    dto.Debug = null;
                    dto.SqlSugerido = null; // Also hide SQL if not admin? User said "Solo Admin" for SQL too in "Solapa 2"
                }

                return Ok(dto);
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
