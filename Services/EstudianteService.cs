using EstudiantesApi.Data;
using EstudiantesApi.Dtos;
using EstudiantesApi.Exceptions;
using EstudiantesApi.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EstudiantesApi.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EstudianteService> _logger;

        public EstudianteService(AppDbContext context, ILogger<EstudianteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<EstudianteResponseDto>> GetAllAsync()
        {
            return await _context.Estudiantes
                .AsNoTracking()
                .ProjectToType<EstudianteResponseDto>()   // Mapster projection
                .ToListAsync();
        }

        public async Task<EstudianteResponseDto?> GetByIdAsync(int id)
        {
            var estudiante = await _context.Estudiantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            return estudiante?.Adapt<EstudianteResponseDto>();
        }

        public async Task<EstudianteResponseDto> CreateAsync(CreateEstudianteDto dto)
        {
            var estudiante = dto.Adapt<Estudiante>();
            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Estudiante creado {Id}", estudiante.Id);
            return estudiante.Adapt<EstudianteResponseDto>();
        }

        public async Task AddCalificacionAsync(int estudianteId, CreateCalificacionDto dto)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null) throw new NotFoundException($"Estudiante {estudianteId} no existe");

            var calif = dto.Adapt<Calificacion>();
            calif.EstudianteId = estudianteId;
            calif.Fecha = DateTime.UtcNow;

            _context.Calificaciones.Add(calif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CalificacionResponseDto>> GetCalificacionesAsync(int estudianteId)
        {
            return await _context.Calificaciones
                .AsNoTracking()
                .Where(c => c.EstudianteId == estudianteId)
                .ProjectToType<CalificacionResponseDto>()
                .ToListAsync();
        }

        public async Task<PagedResultDto<EstudianteResponseDto>> GetPagedAsync(int pageSize, int? cursor = null)
        {
            // Validación básica
            pageSize = Math.Clamp(pageSize, 1, 100); // límite razonable para evitar abuso

            var query = _context.Estudiantes
                .AsNoTracking()
                .OrderBy(e => e.Id);  // Ordenación estable obligatoria

            // Aplicar cursor si existe
            if (cursor.HasValue)
            {
                query = query.Where(e => e.Id > cursor.Value).OrderBy(e => e.Id);
            }

            var items = await query
                .Take(pageSize + 1)  // +1 para saber si hay más
                .ProjectToType<EstudianteResponseDto>()
                .ToListAsync();

            var hasMore = items.Count > pageSize;
            var pagedItems = items.Take(pageSize).ToList();

            int? nextCursor = hasMore ? pagedItems.Last().Id : null;

            return new PagedResultDto<EstudianteResponseDto>(
                pagedItems,
                nextCursor,
                pageSize,
                hasMore
            );
        }
    }
}
