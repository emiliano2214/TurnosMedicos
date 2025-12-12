namespace TurnosMedicos.Models.Dto
{
    public class ChatIaResponseDto
    {
        public string RespuestaUsuario { get; set; } = "";
        public string SqlSugerido { get; set; } // opcional
        public DebugDto Debug { get; set; } // solo para Admin
    }
}
