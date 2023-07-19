using CloudinaryDotNet.Actions;

namespace FirstWebApp.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DetelePhotoAsync(string publicId);
    }
}
