namespace LittleberryApi.Services;

public interface IAzureBlobService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string fileName);
    string GetImageUrl(string fileName);
}
