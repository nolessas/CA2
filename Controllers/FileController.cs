using Microsoft.AspNetCore.Mvc;
using CA1.Database;
using CA1.Database.Entities;
using CA1.Interface;
using CA1.Dto;
using Microsoft.EntityFrameworkCore;

namespace CA1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class FileController : ControllerBase
    {
        private readonly IMyLogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public FileController(IMyLogger logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FileEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllLines()
        {
            try
            {
                _logger.LogInfo("Retrieving all entries");
                var entries = await _dbContext.Files.OrderByDescending(f => f.CreatedAt).ToListAsync();
                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to retrieve logs at this time");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FileEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLine(int id)
        {
            try
            {
                _logger.LogInfo($"Retrieving entry {id}");
                var entry = await _dbContext.Files.FindAsync(id);
                
                if (entry == null)
                {
                    _logger.LogWarning($"Entry {id} not found");
                    return NotFound($"Entry {id} not found");
                }

                return Ok(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to retrieve logs at this time");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(FileEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLine([FromBody] FileRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Content))
                {
                    _logger.LogWarning("Attempted to add empty content");
                    return BadRequest("Content cannot be empty");
                }

                var entity = new FileEntity
                {
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                await _dbContext.Files.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                _logger.LogInfo($"Added new entry: {request.Content}");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to create new log entry");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(FileEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLine(int id, [FromBody] FileRequestDto request)
        {
            try
            {
                var entity = await _dbContext.Files.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"Entry {id} not found for update");
                    return NotFound($"Entry {id} not found");
                }

                entity.Content = request.Content;
                await _dbContext.SaveChangesAsync();

                _logger.LogInfo($"Updated entry {id}: {request.Content}");
                return Ok(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to update log entry");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteLine(int id)
        {
            try
            {
                var entity = await _dbContext.Files.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"Entry {id} not found for deletion");
                    return NotFound($"Entry {id} not found");
                }

                _dbContext.Files.Remove(entity);
                await _dbContext.SaveChangesAsync();

                _logger.LogInfo($"Deleted entry {id}");
                return Ok($"Entry {id} deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal error: {ex.Message}");
                return HandleError("Unable to delete log entry");
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
