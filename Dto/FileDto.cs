using System;

namespace CA1.Dto
{
    // Used for incoming requests when creating or updating a file
    public class FileRequestDto
    {
        // Content of the file to be stored
        // Empty string as default value to avoid null
        public string Content { get; set; } = string.Empty;
    }

    // Used for responses when returning file data to clients
    public class FileResponseDto
    {
        // Unique identifier of the file
        public int Id { get; set; }
        
        // Actual content of the file
        public string Content { get; set; } = string.Empty;
        
        // Timestamp when the file was created
        public DateTime CreatedAt { get; set; }
    }
}