using System.Collections.Generic;
using System.Threading.Tasks;

using TurnosMedicos.Models;

namespace TurnosMedicos.Services
{
    public interface IIaChatService
    {
        Task<IaResponse> AskQuestionAsync(string question, string userId, string userRole);
        Task<List<ChatMessage>> GetChatHistoryAsync(string userId);
    }

    public class IaResponse
    {
        public string Answer { get; set; } = string.Empty;
        public string? SuggestedSql { get; set; }
        public string? Notes { get; set; }
        
        // Debug Info
        public List<string> RulesApplied { get; set; } = new List<string>();
        public double? ConfidenceScore { get; set; }
        public long? RetrievalTimeMs { get; set; }
        public long? GenerationTimeMs { get; set; }

    }
}
