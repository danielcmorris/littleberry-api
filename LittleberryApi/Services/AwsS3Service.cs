using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace LittleberryApi.Services;

public class AwsS3Service : IAwsS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly ILogger<AwsS3Service> _logger;

    public AwsS3Service(IConfiguration configuration, ILogger<AwsS3Service> logger)
    {
        _logger = logger;
        _bucketName = configuration["AWS:BucketName"] ?? "pfsa";

        var accessKey = configuration["AWS:AccessKey"];
        var secretKey = configuration["AWS:SecretKey"];

        _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string key, string contentType)
    {
        try
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = key,
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead,
            };

            using var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return GetFileUrl(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {Key} to S3", key);
            throw;
        }
    }

    public async Task<Stream?> GetFileAsync(string key)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file {Key} from S3", key);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string key)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {Key} from S3", key);
            return false;
        }
    }

    public string GetFileUrl(string key)
    {
        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}
