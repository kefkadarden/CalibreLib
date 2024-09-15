using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace CalibreLib.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IOptions<BlobStorageOptions> options)
        {
            _blobServiceClient = new BlobServiceClient(options.Value.ConnectionString);
            _containerName = options.Value.ContainerName;
        }

        public BlobContainerClient GetBlobContainerClient()
        {
            return _blobServiceClient.GetBlobContainerClient(_containerName);
        }
    }

    public class BlobStorageOptions
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }

}
