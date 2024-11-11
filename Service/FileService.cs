using System.Collections.Generic;
using System;
using CA1.Interface;
using CA1.Database;
using CA1.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CA1.Service
{
    public class FileService : IFileService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMyLogger _logger;

        public FileService(ApplicationDbContext dbContext, IMyLogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public string FilePath { get; set; } = string.Empty;

        public async Task WriteLine(string content)
        {
            var entity = new FileEntity
            {
                Content = content,
                CreatedAt = DateTime.UtcNow
            };
            await _dbContext.Files.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            _logger.LogInfo($"Added new entry: {content}");
        }

        public async Task<string> ReadLine(int id)
        {
            var entity = await _dbContext.Files.FindAsync(id);
            if (entity == null)
                throw new ArgumentException($"Entry with ID {id} not found");
            
            return entity.Content;
        }

        public async Task<IEnumerable<string>> ReadAllLines()
        {
            var entries = await _dbContext.Files
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => f.Content)
                .ToListAsync();
            
            return entries;
        }

        public async Task ReplaceLine(int id, string newContent)
        {
            var entity = await _dbContext.Files.FindAsync(id);
            if (entity == null)
                throw new ArgumentException($"Entry with ID {id} not found");

            entity.Content = newContent;
            await _dbContext.SaveChangesAsync();
            _logger.LogInfo($"Updated entry {id}: {newContent}");
        }

        public async Task RemoveLine(int id)
        {
            var entity = await _dbContext.Files.FindAsync(id);
            if (entity == null)
                throw new ArgumentException($"Entry with ID {id} not found");

            _dbContext.Files.Remove(entity);
            await _dbContext.SaveChangesAsync();
            _logger.LogInfo($"Deleted entry {id}");
        }
    }
}
