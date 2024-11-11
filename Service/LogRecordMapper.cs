using CA1.Database.Entities;
using CA1.Dto;

namespace CA1.Service
{
    public class LogRecordMapper
    {
        public LogResponseDto MapToResponse(LogEntity log)
        {
            return new LogResponseDto
            {
                Id = log.Id,
                Type = log.Type,
                Message = log.Message,
                CreatedAt = log.CreatedAt
            };
        }

        public string MapToString(LogEntity log)
        {
            return $"[{log.Type}] {log.CreatedAt}: {log.Message}";
        }

        public LogEntity MapToEntity(LogRequestDto request)
        {
            return new LogEntity
            {
                Type = request.LogType.ToUpper(),
                Message = request.Message,
                CreatedAt = DateTime.Now
            };
        }
    }
}
