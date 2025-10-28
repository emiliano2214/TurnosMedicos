using Microsoft.AspNetCore.Identity;

public class UsuarioExt : IdentityUser
{
    public string? DisplayName { get; set; }

    // Vínculos opcionales a tablas de dominio
    public int? PacienteId { get; set; }
    public int? MedicoId { get; set; }
}