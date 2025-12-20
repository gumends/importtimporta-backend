using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IS3Service
    {
        Task<string> UploadAsync(IFormFile file);
        Task<List<string>> ListFilesAsync();
        Task DeleteAsync(string key);
    }
}
