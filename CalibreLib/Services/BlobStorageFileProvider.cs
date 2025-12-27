using Azure.Storage.Blobs;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace CalibreLib.Services
{

    public class AzureBlobFileProvider : IFileProvider
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobFileProvider(BlobContainerClient containerClient)
        {
            _containerClient = containerClient;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {

            var blobClient = _containerClient.GetBlobClient(subpath.TrimStart('/'));
            return new AzureBlobFileInfo(blobClient);
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }

    public class AzureBlobFileInfo : IFileInfo
    {
        private readonly BlobClient _blobClient;

        public AzureBlobFileInfo(BlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        public bool Exists => _blobClient.Exists();

        public long Length => _blobClient.GetProperties().Value.ContentLength;

        public string PhysicalPath => null;

        public string Name => _blobClient.Name;

        public DateTimeOffset LastModified => _blobClient.GetProperties().Value.LastModified;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            return _blobClient.OpenRead();
        }
    }

}
