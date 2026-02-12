using EstudiantesApi.Dtos;
using EstudiantesApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EstudiantesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly IEstudianteService _service;
        private readonly IValidator<CreateEstudianteDto> _validator;

        public EstudiantesController(IEstudianteService service, IValidator<CreateEstudianteDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<List<EstudianteResponseDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<EstudianteResponseDto>> GetById(int id)
        {
            var est = await _service.GetByIdAsync(id);
            return est is null ? NotFound() : Ok(est);
        }

        [HttpPost]
        public async Task<ActionResult<EstudianteResponseDto>> Create([FromBody] CreateEstudianteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPost("{id}/calificaciones")]
        public async Task<IActionResult> AddCalificacion(int id, [FromBody] CreateCalificacionDto dto)
        {
            await _service.AddCalificacionAsync(id, dto);
            return NoContent();
        }

        [HttpGet("{id}/calificaciones")]
        public async Task<ActionResult<List<CalificacionResponseDto>>> GetCalificaciones(int id)
            => Ok(await _service.GetCalificacionesAsync(id));

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<EstudianteResponseDto>>> GetPaged(
            [FromQuery] int pageSize = 20,
            [FromQuery] int? cursor = null)
        {
            var result = await _service.GetPagedAsync(pageSize, cursor);
            return Ok(result);
        }
    }
}
