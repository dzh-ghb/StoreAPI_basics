namespace Api.Services.StorageS3
{
    public interface IFileStorageService
    {
        Task<bool> RemoveFileAsync(string fileName);
        Task<string> UploadFileAsync(IFormFile file);
    }
}