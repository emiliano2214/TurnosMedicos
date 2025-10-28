using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TurnosMedicos.Controllers
{
    public abstract class GenericController<TEntity> : Controller where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericController(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        // GET: Listado
        public virtual async Task<IActionResult> Index()
        {
            var data = await _dbSet.ToListAsync();
            return View(data);
        }

        // GET: Crear
        public virtual IActionResult Crear()
        {
            return View();
        }

        // POST: Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Crear(TEntity entity)
        {
            if (ModelState.IsValid)
            {
                _dbSet.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        // GET: Editar
        public virtual async Task<IActionResult> Editar(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        // POST: Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Editar(int id, TEntity entity)
        {
            if (!ModelState.IsValid) return View(entity);

            _context.Update(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Eliminar
        public virtual async Task<IActionResult> Borrar(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return NotFound();

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
