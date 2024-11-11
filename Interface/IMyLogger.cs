namespace CA1.Interface
{
    public interface IMyLogger
    {
        Task LogInfo(string message);
        Task LogWarning(string message);
        Task LogError(string message);
        Task DeleteLogEntry(string message);
        Task<List<string>> GetAllLogs();
        Task<List<string>> GetLogsByType(string type);
        Task<List<string>> GetLogsByTimeRange(DateTime? startDate, DateTime? endDate);
        Task<string?> GetLogById(int id);
        Task UpdateLog(int id, string type, string message);
        Task DeleteLogsByTimeRange(DateTime? startDate, DateTime? endDate);
    }
}
