namespace LittleberryApi.Services;

public interface IAwsS3Service
{
    Task<string> UploadFileAsync(Stream fileStream, string key, string contentType);
    Task<Stream?> GetFileAsync(string key);
    Task<bool> DeleteFileAsync(string key);
    string GetFileUrl(string key);
}
