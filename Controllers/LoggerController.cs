using Microsoft.AspNetCore.Mvc;
using CA1.Interface;
using CA1.Service;
using CA1.Database;
using CA1.Dto;
using Microsoft.EntityFrameworkCore;

namespace CA1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class LoggerController : ControllerBase
    {
        private readonly IMyLogger _logger;
        private readonly LogRecordMapper _mapper;
        private readonly ApplicationDbContext _context;

        public LoggerController(IMyLogger logger, LogRecordMapper mapper, ApplicationDbContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllLogs()
        {
            try
            {
                var logs = _logger.GetAllLogs();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to retrieve logs at this time");
            }
        }

        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLogsByType(string type)
        {
            try
            {
                var logs = _logger.GetLogsByType(type);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to retrieve logs for the specified type");
            }
        }

        [HttpGet("timerange")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLogsByTimeRange([FromQuery] LogTimeRangeDto request, [FromQuery] string? type = null)
        {
            try
            {
                var query = _context.Logs.AsQueryable();

                if (request.StartDate.HasValue)
                    query = query.Where(l => l.CreatedAt >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(l => l.CreatedAt <= request.EndDate.Value);

                if (!string.IsNullOrEmpty(type))
                    query = query.Where(l => l.Type == type.ToUpper());

                var logs = query
                    .OrderByDescending(l => l.CreatedAt)
                    .Select(l => _mapper.MapToResponse(l))
                    .ToList();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to retrieve logs for the specified time range");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateLog([FromBody] LogRequestDto request)
        {
            try
            {
                switch (request.LogType.ToUpper())
                {
                    case "INFO":
                        _logger.LogInfo(request.Message);
                        break;
                    case "WARNING":
                        _logger.LogWarning(request.Message);
                        break;
                    case "ERROR":
                        _logger.LogError(request.Message);
                        break;
                    default:
                        _logger.LogWarning($"Attempted to create log with invalid type: {request.LogType}");
                        return HandleError("Invalid log type provided");
                }

                return Ok("Log created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to create new log entry");
            }
        }

        [HttpDelete("timerange")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteLogsByTimeRange([FromQuery] LogTimeRangeDto request, [FromQuery] string? type = null)
        {
            try
            {
                if (!request.StartDate.HasValue && !request.EndDate.HasValue)
                {
                    return BadRequest("At least one date parameter is required");
                }

                var query = _context.Logs.AsQueryable();

                if (request.StartDate.HasValue)
                    query = query.Where(l => l.CreatedAt >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(l => l.CreatedAt <= request.EndDate.Value);

                if (!string.IsNullOrEmpty(type))
                    query = query.Where(l => l.Type == type.ToUpper());

                var logsToDelete = query.ToList();
                if (logsToDelete.Any())
                {
                    _context.Logs.RemoveRange(logsToDelete);
                    _context.SaveChanges();
                }

                return Ok($"Logs deleted successfully for period: {request.StartDate} to {request.EndDate}" + 
                         (!string.IsNullOrEmpty(type) ? $" with type {type}" : ""));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to delete logs for the specified time range");
            }
        }

        private IActionResult HandleError(string userMessage)
        {
            return StatusCode(500, new ErrorResponseDto 
            { 
                Message = userMessage
            });
        }
    }
}
