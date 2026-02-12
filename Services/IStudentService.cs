using EstudiantesApi.Dtos;

namespace EstudiantesApi.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto?> GetByIdAsync(int id);
        Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
        Task AddQualificationAsync(int studentId, CreateQualificationDto dto);
        Task<List<QualificationResponseDto>> GetQualificationsAsync(int studentId);
        Task<PagedResultDto<StudentResponseDto>> GetPagedAsync(int pageSize, int? cursor = null);
    }
}
