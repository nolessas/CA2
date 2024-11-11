namespace CA1.Interface
{
    public interface IFileService
    {
        string FilePath { get; set; }
        Task WriteLine(string content);
        Task<string> ReadLine(int id);
        Task<IEnumerable<string>> ReadAllLines();
        Task ReplaceLine(int id, string newContent);
        Task RemoveLine(int id);
    }
}
