using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options; 
using MortgageComparerAPI.Services.Interfaces;

namespace MortgageComparerAPI.Services {

    public class AzureStorageSettings {
        public string ContainerName { get; set; } = string.Empty;
    }

    public class AzureBlobStorageService : IFileStorageService {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;


        public AzureBlobStorageService(BlobServiceClient blobServiceClient, IOptions<AzureStorageSettings> options) {
            _blobServiceClient = blobServiceClient;
            _containerName = options.Value.ContainerName;

            if (string.IsNullOrWhiteSpace(_containerName)) {
                throw new ArgumentNullException(nameof(options), "ContainerName is not configured.");
            }
                
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType = "application/octet-stream") {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);


            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
           

            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            if (fileStream.CanSeek) {
                fileStream.Position = 0;
            }

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            });

            return blobClient.Uri.ToString();
        }

        public async Task<(Stream? FileStream, string ContentType)?> DownloadAsync(string fileUrlOrName) {
            string fileName = ExtractFileName(fileUrlOrName);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync()) {
                return null;
            }

            var downloadInfo = await blobClient.DownloadAsync(CancellationToken.None);

            return (downloadInfo.Value.Content, downloadInfo.Value.ContentType);
        }

        public async Task DeleteAsync(string fileUrlOrName) {
            string fileName = ExtractFileName(fileUrlOrName);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteIfExistsAsync();
        }

        private string ExtractFileName(string input) {
            if (Uri.TryCreate(input, UriKind.Absolute, out var uri)) {
                return Path.GetFileName(uri.LocalPath);
            }
            return input;
        }
    }
}