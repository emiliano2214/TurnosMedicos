using System.ComponentModel.DataAnnotations;

public class RegisterUsuarioViewModel
{
    // Datos de login
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    // Datos básicos
    [Required]
    public string Nombre { get; set; } = null!;

    [Required]
    public string Apellido { get; set; } = null!;

    [Required]
    public string RolSeleccionado { get; set; } = null!;
    // "Paciente" o "Medico"

    // Campos comunes
    public string? Telefono { get; set; }

    // Campos para Paciente
    public string? Dni { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public int? IdObraSocial { get; set; }

    // Campos para Médico
    public string? Matricula { get; set; }
    public int? IdEspecialidad { get; set; }
}
