
namespace PermitService.Helpers
{
    public interface IFileProvider
    {
        Task<List<string>> ReadLines(string filePath);
    }
}