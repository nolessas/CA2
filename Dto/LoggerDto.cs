using System.ComponentModel.DataAnnotations;

namespace CA1.Dto
{
    // Used when creating new log entries
    public class LogRequestDto
    {
        [Required]
        public string Message { get; set; } = string.Empty; // Message to be logged
        
        [Required]
        public string LogType { get; set; } = "INFO"; // Default log type
    }

    // Used when returning log entries to clients
    public class LogResponseDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Used for filtering logs by date range
    public class LogTimeRangeDto
    {
        public string? LogType { get; set; }        // Optional filter by log type
        public DateTime? StartDate { get; set; }    // Optional start date
        public DateTime? EndDate { get; set; }      // Optional end date
    }
}

// Used for error responses
public class ErrorResponseDto
{
    public string Message { get; set; }
}