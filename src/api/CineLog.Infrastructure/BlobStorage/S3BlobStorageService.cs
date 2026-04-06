using Amazon.S3;
using Amazon.S3.Model;
using CineLog.Application.Common;
using Microsoft.Extensions.Options;

namespace CineLog.Infrastructure.BlobStorage;

public class S3BlobStorageService : IBlobStorageService
{
    private readonly IAmazonS3 _s3;
    private readonly BlobStorageOptions _options;

    public S3BlobStorageService(IAmazonS3 s3, IOptions<BlobStorageOptions> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(string key, Stream content, string contentType, CancellationToken ct = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3.PutObjectAsync(request, ct);

        return $"{_options.PublicBaseUrl.TrimEnd('/')}/{key}";
    }

    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        };

        await _s3.DeleteObjectAsync(request, ct);
    }
}
