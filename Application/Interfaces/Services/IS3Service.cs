using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Services;

public interface IS3Service
{
    Task<string> UploadAsync(IFormFile file);
    Task<List<string>> ListFilesAsync();
    Task DeleteAsync(string key);
}
