using EstudiantesApi.Dtos;
using EstudiantesApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EstudiantesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;
        private readonly IValidator<CreateStudentDto> _validator;

        public StudentsController(IStudentService service, IValidator<CreateStudentDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentResponseDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentResponseDto>> GetById(int id)
        {
            var est = await _service.GetByIdAsync(id);
            return est is null ? NotFound() : Ok(est);
        }

        [HttpPost]
        public async Task<ActionResult<StudentResponseDto>> Create([FromBody] CreateStudentDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id}/qualifications")]
        public async Task<IActionResult> AddQualification(int id, [FromBody] CreateQualificationDto dto)
        {
            await _service.AddQualificationAsync(id, dto);
            return NoContent();
        }

        [HttpGet("{id}/qualifications")]
        public async Task<ActionResult<List<QualificationResponseDto>>> GetQualifications(int id)
            => Ok(await _service.GetQualificationsAsync(id));

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<StudentResponseDto>>> GetPaged(
            [FromQuery] int pageSize = 20,
            [FromQuery] int? cursor = null)
        {
            var result = await _service.GetPagedAsync(pageSize, cursor);
            return Ok(result);
        }
    }
}
