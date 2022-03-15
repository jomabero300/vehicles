using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Vehicles.API.Helpers
{
    public interface IImageHelper
    {
        Task<Guid> UploadImageAsync(IFormFile ProFile, string Folder);
        Task<string> DeleteImageAsync(string File, string Folder);
        string DeleteImageAsync(string File);
    }
}
