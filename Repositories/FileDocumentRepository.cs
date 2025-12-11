using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace TurnosMedicos.Repositories
{
    public class FileDocumentRepository : IDocumentRepository
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _docsPath;

        public FileDocumentRepository(IWebHostEnvironment env)
        {
            _env = env;
            // Assumes docs are in a known location relative to content root.
            // Adjust path as needed based on project structure. 
            // The user said: "iamodel/docs"
            _docsPath = Path.Combine(_env.ContentRootPath, "iamodel", "docs");
        }

        public async Task<string> GetCombinedContextAsync()
        {
            if (!Directory.Exists(_docsPath))
            {
                return "No documentation found.";
            }

            var sb = new StringBuilder();
            var files = Directory.GetFiles(_docsPath, "*.md");

            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                sb.AppendLine($"--- Start of {Path.GetFileName(file)} ---");
                sb.AppendLine(content);
                sb.AppendLine($"--- End of {Path.GetFileName(file)} ---");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
