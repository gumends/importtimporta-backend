using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace Application.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3;
    private readonly S3Settings _settings;

    public S3Service(IOptions<S3Settings> options)
    {
        _settings = options.Value;

        _s3 = new AmazonS3Client(
            _settings.AccessKey,
            _settings.SecretKey,
            RegionEndpoint.GetBySystemName(_settings.Region)
        );
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Arquivo inválido");

        var fileName =
            $"produtos/{Guid.NewGuid()}_{file.FileName.Replace(" ", "_")}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3.PutObjectAsync(request);

        return $"https://{_settings.BucketName}.s3.amazonaws.com/{fileName}";
    }

    public async Task<List<string>> ListFilesAsync()
    {
        var response = await _s3.ListObjectsV2Async(new ListObjectsV2Request
        {
            BucketName = _settings.BucketName
        });

        return response.S3Objects
            .Select(o => $"https://{_settings.BucketName}.s3.amazonaws.com/{o.Key}")
            .ToList();
    }

    public async Task DeleteAsync(string key)
    {
        await _s3.DeleteObjectAsync(_settings.BucketName, key);
    }
}
