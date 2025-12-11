using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using TurnosMedicos.Repositories;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TurnosMedicos.Services
{
    public class IaChatService : IIaChatService
    {
        private readonly IDocumentRepository _docRepo;
        private readonly string _apiKey;
        private readonly string _model = "gpt-4o-mini"; // Or configurable

        public IaChatService(IDocumentRepository docRepo, IConfiguration config)
        {
            _docRepo = docRepo;
            _apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey not found in configuration");
        }

        public async Task<IaResponse> AskQuestionAsync(string question, string userRole)
        {
            var context = await _docRepo.GetCombinedContextAsync();

            var systemPrompt = $@"
Eres un asistente de IA 'Tipo 2' integrado en un sistema de turnos médicos (ASP.NET MVC).
Tu objetivo es ayudar al usuario ({userRole}) respondiendo preguntas, explicando reglas y, si es solicitado, generando SQL sugerido (solo lectura) para SQL Server.
Usa la siguiente documentación interna para responder:
{context}

REGLAS:
1. Responde NO solo en base a la documentación. Si no sabes, dilo.
2. Si el usuario pide una consulta de base de datos, genera una query SQL válida para SQL Server en el campo 'suggested_sql'.
3. Tu respuesta debe ser EXCLUSIVAMENTE un objeto JSON con este formato:
{{
  ""answer"": ""Texto de la respuesta para el usuario"",
  ""suggested_sql"": ""SELECT ... (o null si no aplica)"",
  ""notes"": ""Notas adicionales o null""
}}
4. No incluyas markdown (```json) rodeando la respuesta, solo el JSON crudo.
";

            ChatClient client = new ChatClient(_model, _apiKey);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"Rol: {userRole}. Pregunta: {question}")
            };

            ChatCompletion completion = await client.CompleteChatAsync(messages);

            var responseText = completion.Content[0].Text;

            // Clean up if markdown was included despite instructions
            responseText = responseText.Replace("```json", "").Replace("```", "").Trim();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var responseObj = JsonSerializer.Deserialize<IaResponse>(responseText, options);
                return responseObj ?? new IaResponse { Answer = "Error parsing AI response." };
            }
            catch (Exception ex)
            {
                // Fallback if JSON fails
                return new IaResponse
                {
                    Answer = $"Error al procesar la respuesta de la IA. Raw: {responseText}",
                    Notes = ex.Message
                };
            }
        }
    }
}
