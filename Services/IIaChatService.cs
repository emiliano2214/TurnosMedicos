using System.Threading.Tasks;

namespace TurnosMedicos.Services
{
    public interface IIaChatService
    {
        Task<IaResponse> AskQuestionAsync(string question, string userRole);
    }

    public class IaResponse
    {
        public string Answer { get; set; } = string.Empty;
        public string? SuggestedSql { get; set; }
        public string? Notes { get; set; }
    }
}
