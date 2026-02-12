using EstudiantesApi.Dtos;

namespace EstudiantesApi.Services
{
    public interface IEstudianteService
    {
        Task<List<EstudianteResponseDto>> GetAllAsync();
        Task<EstudianteResponseDto?> GetByIdAsync(int id);
        Task<EstudianteResponseDto> CreateAsync(CreateEstudianteDto dto);
        Task AddCalificacionAsync(int estudianteId, CreateCalificacionDto dto);
        Task<List<CalificacionResponseDto>> GetCalificacionesAsync(int estudianteId);
        Task<PagedResultDto<EstudianteResponseDto>> GetPagedAsync(int pageSize, int? cursor = null);
    }
}
