using System.Collections.Generic;

namespace TurnosMedicos.Models.Dto
{
    public class DebugDto
    {
        public List<string> Documentos { get; set; } = new List<string>();
        public List<string> ReglasAplicadas { get; set; } = new List<string>();
        public double? Confianza { get; set; }
        public string PromptFinal { get; set; }
        public long? MsRetrieval { get; set; }
        public long? MsGeneracion { get; set; }
    }
}
