using StudentsApi.Data;
using StudentsApi.Dtos;
using StudentsApi.Exceptions;
using StudentsApi.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace StudentsApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(AppDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<StudentResponseDto>> GetAllAsync()
        {
            return await _context.Students
                .AsNoTracking()
                .ProjectToType<StudentResponseDto>()   // Mapster projection
                .ToListAsync();
        }

        public async Task<StudentResponseDto?> GetByIdAsync(int id)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            return student?.Adapt<StudentResponseDto>();
        }

        public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
        {
            var student = dto.Adapt<Student>();
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Student created {Id}", student.Id);
            return student.Adapt<StudentResponseDto>();
        }

        public async Task AddQualificationAsync(int studentId, CreateQualificationDto dto)
        {
            var estudiante = await _context.Students.FindAsync(studentId);
            if (estudiante == null) throw new NotFoundException($"Student {studentId} doesnt exist");

            var calif = dto.Adapt<Qualification>();
            calif.StudentId = studentId;
            calif.Grade = dto.Grade;
            calif.Date = DateTime.UtcNow;

            _context.Qualifications.Add(calif);
            await _context.SaveChangesAsync();
        }

        public async Task<List<QualificationResponseDto>> GetQualificationsAsync(int studentId)
        {
            return await _context.Qualifications
                .AsNoTracking()
                .Where(c => c.StudentId == studentId)
                .ProjectToType<QualificationResponseDto>()
                .ToListAsync();
        }

        public async Task<PagedResultDto<StudentResponseDto>> GetPagedAsync(int pageSize, int? cursor = null)
        {
            // Basic validation
            pageSize = Math.Clamp(pageSize, 1, 100); // reasonable limit to prevent abuse

            var query = _context.Students
                .AsNoTracking()
                .OrderBy(e => e.Id); // stable ordering is mandatory for reliable cursor pagination

            // Apply cursor if provided
            if (cursor.HasValue)
            {
                query = query
                    .Where(e => e.Id > cursor.Value)
                    .OrderBy(e => e.Id);
            }

            var items = await query
                .Take(pageSize + 1) // +1 to detect if there are more pages
                .ProjectToType<StudentResponseDto>()
                .ToListAsync();

            var hasMore = items.Count > pageSize;
            var pagedItems = items.Take(pageSize).ToList();

            int? nextCursor = hasMore ? pagedItems.Last().Id : null;

            return new PagedResultDto<StudentResponseDto>(
                pagedItems,
                nextCursor,
                pageSize,
                hasMore
            );
        }
    }
}
