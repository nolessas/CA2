using System;

namespace CA1.Database.Entities
{
    public class FileEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}