using CA1.Database;
using CA1.Database.Entities;
using CA1.Interface;
using Microsoft.EntityFrameworkCore;

namespace CA1.Service
{
    /// <summary>
    /// Database logger implementation. This service should be registered as Scoped
    /// because it depends on DbContext which is scoped.
    /// 
    /// Lifetime explanations:
    /// - Singleton: Single instance throughout application lifetime
    /// - Scoped: New instance per HTTP request
    /// - Transient: New instance every time requested
    /// 
    /// For database operations, Scoped is preferred as it aligns with
    /// Entity Framework's DbContext lifetime.
    /// </summary>
    public class MyLogger : IMyLogger
    {
        private readonly ApplicationDbContext _context;

        public MyLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogInfo(string message)
        {
            await CreateLog("INFO", message);
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
        }

        public async Task LogWarning(string message)
        {
            await CreateLog("WARNING", message);
            Console.WriteLine($"[WARNING] {DateTime.Now}: {message}");
        }

        public async Task LogError(string message)
        {
            await CreateLog("ERROR", message);
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
        }

        public async Task DeleteLogEntry(string message)
        {
            await CreateLog("DELETE", message);
            Console.WriteLine($"[DELETE] {DateTime.Now}: {message}");
        }

        public async Task<List<string>> GetAllLogs()
        {
            var logs = await _context.Logs
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => $"[{l.Type}] {l.CreatedAt}: {l.Message}")
                .ToListAsync();
            return logs;
        }

        public async Task<List<string>> GetLogsByType(string type)
        {
            var logs = await _context.Logs
                .Where(l => l.Type == type.ToUpper())
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => $"[{l.Type}] {l.CreatedAt}: {l.Message}")
                .ToListAsync();
            return logs;
        }

        public async Task<List<string>> GetLogsByTimeRange(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Logs.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(l => l.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.CreatedAt <= endDate.Value);

            var logs = await query
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => $"[{l.Type}] {l.CreatedAt}: {l.Message}")
                .ToListAsync();
            return logs;
        }

        public async Task<string?> GetLogById(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null) return null;
            return $"[{log.Type}] {log.CreatedAt}: {log.Message}";
        }

        public async Task UpdateLog(int id, string type, string message)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                log.Type = type.ToUpper();
                log.Message = message;
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateLog(string type, string message)
        {
            var log = new LogEntity
            {
                Type = type,
                Message = message,
                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogsByTimeRange(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Logs.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(l => l.CreatedAt >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(l => l.CreatedAt <= endDate.Value);

                var logsToDelete = await query.ToListAsync();
                if (logsToDelete.Any())
                {
                    _context.Logs.RemoveRange(logsToDelete);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                Console.WriteLine($"Error deleting logs: {ex.Message}");
                throw;
            }
        }
    }
}
