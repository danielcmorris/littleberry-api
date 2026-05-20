using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace LittleberryApi.Services;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string _imageRootPath;
    private readonly ILogger<AzureBlobService> _logger;

    public AzureBlobService(IConfiguration configuration, ILogger<AzureBlobService> logger)
    {
        _logger = logger;
        var connectionString = configuration.GetConnectionString("BlobStorage");
        _containerName = configuration["AppSettings:ImagesContainer"] ?? "bookimages";
        _imageRootPath = configuration["AppSettings:ImageRootPath"] ?? "";

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            return GetImageUrl(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload image {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete image {FileName}", fileName);
            return false;
        }
    }

    public string GetImageUrl(string fileName)
    {
        return $"{_imageRootPath}/{fileName}";
    }
}
