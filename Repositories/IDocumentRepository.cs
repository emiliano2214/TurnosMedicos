using System.Collections.Generic;
using System.Threading.Tasks;

namespace TurnosMedicos.Repositories
{
    public interface IDocumentRepository
    {
        Task<string> GetCombinedContextAsync();
    }
}
