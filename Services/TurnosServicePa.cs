using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Data;
using TurnosMedicos.Models.ViewModels;

namespace TurnosMedicos.Services
{
    public class TurnosServicePa
    {
        private readonly ApplicationDbContext _context;

        public TurnosServicePa(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitarTurnoResultado> SolicitarTurnoAsync(int idPaciente, int idEspecialidad, DateTime fechaAtencion)
        {
            var resultado = await _context.Set<SolicitarTurnoResultado>()
                .FromSqlInterpolated($"EXEC dbo.sp_SolicitarTurno {idPaciente}, {idEspecialidad}, {fechaAtencion}")
                .ToListAsync();

            if (resultado.Count == 0)
                throw new Exception("No se pudo generar el turno.");

            return resultado.First();
        }
    }
}
