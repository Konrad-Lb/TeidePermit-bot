
namespace PermitService.Helpers
{
    public interface IFileProvider
    {
        Task<IEnumerable<string>> ReadLines(string filePath);
        bool FileExists(string filePath);
        void DeleteFile(string filePath);

        Task WriteLines(string filePath, IEnumerable<string> lines);
    }
}