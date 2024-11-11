using System;

namespace CA1.Database.Entities
{
    public class LogEntity
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
} 