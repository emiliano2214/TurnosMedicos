using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosMedicos.Repositories;

namespace TurnosMedicos.Services
{
    /// <summary>
    /// Asistente IA local (sin OpenAI), basado en:
    /// - Reglas por palabras clave
    /// - Documentación interna en iamodel/docs (vía IDocumentRepository)
    /// </summary>
    public class IaChatService : IIaChatService
    {
        private readonly IDocumentRepository _docRepo;

        public IaChatService(IDocumentRepository docRepo)
        {
            _docRepo = docRepo;
        }

        public async Task<IaResponse> AskQuestionAsync(string question, string userRole)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var response = new IaResponse();
            
            // Normalizar entrada
            if (string.IsNullOrWhiteSpace(question))
            {
                response.Answer = "No recibí ninguna pregunta. Probá escribiendo algo como: \"¿Cómo saco un turno?\"";
                response.RulesApplied.Add("Input Validation: Empty Question");
                return response;
            }

            var normalized = Normalizar(question);

            // Cargar contexto completo desde iamodel/docs
            var swRetrieval = System.Diagnostics.Stopwatch.StartNew();
            var fullContext = await _docRepo.GetCombinedContextAsync();
            swRetrieval.Stop();
            response.RetrievalTimeMs = swRetrieval.ElapsedMilliseconds;

            var rag = new LocalRagEngine(fullContext);

            // ==============
            //  REGLAS BASE
            // ==============

            // 1) Saludos
            if (ContieneAlguna(normalized, "hola", "buen dia", "buenas", "que tal"))
            {
                var saludo = new StringBuilder();
                saludo.AppendLine("¡Hola! Soy el asistente del sistema de turnos médicos.");
                saludo.AppendLine("Podés preguntarme cosas como:");
                saludo.AppendLine("- \"¿Cómo saco un turno?\"");
                saludo.AppendLine("- \"¿Qué roles hay en el sistema?\"");
                saludo.AppendLine("- \"¿Qué puede hacer mi rol?\"");

                response.Answer = saludo.ToString();
                response.Notes = rag.Retrieve("casos de uso");
                response.RulesApplied.Add("Regla: Saludos");
                response.ConfidenceScore = 1.0;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 2) Preguntas sobre cómo sacar / hacer un turno
            if (ContieneAlguna(normalized, "como sacar un turno", "como hago un turno", "sacar turno", "pedir turno", "solicitar turno"))
            {
                var respuesta = new StringBuilder();
                respuesta.AppendLine("Para sacar un turno en el sistema:");
                respuesta.AppendLine("1. Iniciá sesión con tu usuario y contraseña.");
                respuesta.AppendLine("2. Entrá al menú \"Turnos\".");
                respuesta.AppendLine("3. Elegí médico, especialidad y fecha disponible.");
                respuesta.AppendLine("4. Confirmá el turno para que quede registrado.");

                if (userRole.Equals("Paciente", StringComparison.OrdinalIgnoreCase))
                {
                    respuesta.AppendLine();
                    respuesta.AppendLine("Como *Paciente*, solo podés solicitar turnos para vos mismo.");
                }
                else if (userRole.Equals("Administrativo", StringComparison.OrdinalIgnoreCase))
                {
                    respuesta.AppendLine();
                    respuesta.AppendLine("Como *Administrativo*, podés gestionar turnos para distintos pacientes desde las vistas de administración.");
                }

                response.Answer = respuesta.ToString();
                response.SuggestedSql = "EXEC SolicitarTurno @IdPaciente, @IdMedico, @Fecha, @Hora;";
                response.Notes = rag.Retrieve("reglas negocio turnos estados_turno ejemplos_sql");
                response.RulesApplied.Add("Regla: Procedimiento Turnos");
                response.ConfidenceScore = 0.95;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 3) Preguntas sobre roles / permisos
            if (ContieneAlguna(normalized, "rol", "roles del sistema", "permisos", "que puedo hacer", "que hace el rol"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("En el sistema existen estos roles principales:");
                sb.AppendLine("- Admin: administra usuarios, roles y configuraciones generales.");
                sb.AppendLine("- Administrativo: gestiona pacientes, turnos, obras sociales y consultorios.");
                sb.AppendLine("- Médico: puede ver sus turnos, historias clínicas y registrar atenciones.");
                sb.AppendLine("- Paciente: puede ver su perfil y solicitar turnos.");
                sb.AppendLine();
                sb.AppendLine($"Tu rol actual es: {userRole}.");

                response.Answer = sb.ToString();
                response.Notes = rag.Retrieve("roles permisos casos_de_uso");
                response.RulesApplied.Add("Regla: Info Roles");
                response.ConfidenceScore = 0.95;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 4) Preguntas sobre estados de turno
            if (ContieneAlguna(normalized, "estado del turno", "estados de turno", "pendiente", "confirmado", "cancelado"))
            {
                response.Answer = "Los estados de turno más comunes son: Pendiente, Confirmado, Atendido y Cancelado. " +
                                  "Cada estado indica en qué punto del flujo está la atención del paciente.";
                response.Notes = rag.Retrieve("estados_turno reglas_negocio_turnos");
                response.RulesApplied.Add("Regla: Estados Turno");
                response.ConfidenceScore = 0.9;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 5) Preguntas relacionadas con SQL / base de datos
            if (ContieneAlguna(normalized, "sql", "consulta sql", "base de datos", "tabla", "query"))
            {
                var respuesta = new StringBuilder();
                respuesta.AppendLine("Puedo sugerirte consultas SQL de solo lectura sobre la base de datos de turnos.");
                respuesta.AppendLine("Por ejemplo, para ver todos los turnos pendientes:");
                respuesta.AppendLine("SELECT * FROM Turnos WHERE Estado = 'Pendiente';");

                response.Answer = respuesta.ToString();
                response.SuggestedSql = "SELECT * FROM Turnos WHERE Estado = 'Pendiente';";
                response.Notes = rag.Retrieve("ejemplos_sql modelo_datos_oltp");
                response.RulesApplied.Add("Regla: Ayuda SQL");
                response.ConfidenceScore = 0.85;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 6) Preguntas genéricas que mencionan "turno"
            if (normalized.Contains("turno"))
            {
                response.Answer = "Parece que tu pregunta está relacionada con turnos. " +
                                  "Podés ser más específico, por ejemplo: \"¿Cómo saco un turno?\" o \"¿Qué significa turno pendiente?\"";
                response.Notes = rag.Retrieve("reglas_negocio_turnos estados_turno");
                response.RulesApplied.Add("Regla: Detección Genérica Turno");
                response.ConfidenceScore = 0.6;
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 7) Fallback: usar RAG sobre la documentación
            var retrieved = rag.Retrieve(normalized);

            if (!string.IsNullOrWhiteSpace(retrieved))
            {
                response.Answer = "Encontré información relacionada en la documentación interna. A continuación te muestro un resumen:";
                response.Notes = retrieved;
                response.RulesApplied.Add("Fallback: RAG Search");
                response.ConfidenceScore = 0.5; // Aproximado
                
                sw.Stop();
                response.GenerationTimeMs = sw.ElapsedMilliseconds;
                return response;
            }

            // 8) Último recurso: respuesta genérica
            response.Answer = "Todavía no tengo una respuesta específica para esa pregunta. " +
                              "Probá reformularla o consultá al administrador del sistema.";
            response.RulesApplied.Add("Fallback: No Match");
            response.ConfidenceScore = 0.0;
            
            sw.Stop();
            response.GenerationTimeMs = sw.ElapsedMilliseconds;
            return response;
        }

        // ==========================
        // Helpers de texto
        // ==========================

        private static string Normalizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            texto = texto.ToLowerInvariant().Trim();

            var sb = new StringBuilder();
            foreach (var c in texto)
            {
                sb.Append(c switch
                {
                    'á' => 'a',
                    'é' => 'e',
                    'í' => 'i',
                    'ó' => 'o',
                    'ú' => 'u',
                    'ñ' => 'ñ',
                    _ => c
                });
            }

            return sb.ToString();
        }

        private static bool ContieneAlguna(string texto, params string[] frases)
        {
            if (string.IsNullOrWhiteSpace(texto)) return false;
            return frases.Any(f => texto.Contains(f));
        }

        // ==========================
        // Motor RAG local muy simple
        // ==========================
        private class LocalRagEngine
        {
            private readonly string _context;

            public LocalRagEngine(string fullContext)
            {
                _context = fullContext?.ToLowerInvariant() ?? string.Empty;
            }

            /// <summary>
            /// Devuelve un "snippet" de texto de la documentación
            /// que contenga alguna de las palabras clave de la consulta.
            /// </summary>
            public string? Retrieve(string query)
            {
                if (string.IsNullOrWhiteSpace(_context) ||
                    string.IsNullOrWhiteSpace(query))
                    return null;

                var q = query.ToLowerInvariant();
                var keywords = q.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Where(w => w.Length > 3)
                                .Distinct()
                                .Take(5)
                                .ToList();

                if (!keywords.Any())
                    return null;

                var sb = new StringBuilder();

                foreach (var key in keywords)
                {
                    var snippet = ExtraerParrafo(_context, key, 400);
                    if (!string.IsNullOrWhiteSpace(snippet))
                    {
                        sb.AppendLine($"• Coincidencia por \"{key}\":");
                        sb.AppendLine(snippet.Trim());
                        sb.AppendLine();
                    }
                }

                return sb.Length > 0 ? sb.ToString() : null;
            }

            private static string ExtraerParrafo(string texto, string palabra, int radio)
            {
                int idx = texto.IndexOf(palabra, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) return string.Empty;

                int start = Math.Max(0, idx - radio);
                int end = Math.Min(texto.Length, idx + radio);

                return texto.Substring(start, end - start);
            }
        }
    }
}
