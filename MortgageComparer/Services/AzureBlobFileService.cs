using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Services {
    public class AzureBlobStorageService : IFileStorageService {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration) {
            _blobServiceClient = blobServiceClient;
            _containerName = configuration["AzureStorage:ContainerName"]
                             ?? throw new ArgumentNullException("Nie znaleziono ContainerName w konfiguracji.");
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType = "application/pdf") {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

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

        public async Task<byte[]?> DownloadAsync(string fileUrlOrName) {
            string fileName = ExtractFileName(fileUrlOrName);

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync()) {
                using var ms = new MemoryStream();
                await blobClient.DownloadToAsync(ms);
                return ms.ToArray();
            }

            return null;
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
            // Jeśli to zwykły string (sama nazwa), zwracamy go
            return input;
        }
    }
}