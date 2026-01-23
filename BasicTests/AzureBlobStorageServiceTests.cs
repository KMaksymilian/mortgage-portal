using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using Moq;
using MortgageComparer.Services;
using Xunit;

namespace MortgageComparer.Tests.Services {
    public class AzureBlobStorageServiceTests {
        private readonly Mock<BlobServiceClient> _blobServiceClientMock;
        private readonly Mock<BlobContainerClient> _containerClientMock;
        private readonly Mock<BlobClient> _blobClientMock;
        private readonly Mock<IOptions<AzureStorageSettings>> _optionsMock;

        private readonly AzureBlobStorageService _service;
        private const string TestContainerName = "test-container";

        public AzureBlobStorageServiceTests() {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _containerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();


            _blobServiceClientMock
                .Setup(x => x.GetBlobContainerClient(TestContainerName))
                .Returns(_containerClientMock.Object);

            _containerClientMock
                .Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(_blobClientMock.Object);

            _optionsMock = new Mock<IOptions<AzureStorageSettings>>();
            _optionsMock.Setup(x => x.Value).Returns(new AzureStorageSettings {
                ContainerName = TestContainerName
            });

            _service = new AzureBlobStorageService(_blobServiceClientMock.Object, _optionsMock.Object);
        }

        [Fact]
        public async Task Upload_ShouldCreateContainerAndUploadFile_Async() {
            // Arrange
            var fileName = "test.pdf";
            var fakeUri = new Uri("http://azure.mock/test-container/guid_test.pdf");
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

            _blobClientMock.SetupGet(x => x.Uri).Returns(fakeUri);

            _blobClientMock
                .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.BlobContentInfo(ETag.All, DateTimeOffset.Now, null, null, null, 123), null!));

            // Act
            var result = await _service.UploadAsync(stream, fileName);

            // Assert
          
            _containerClientMock.Verify(x => x.CreateIfNotExistsAsync(PublicAccessType.Blob, null, null, default), Times.Once);
            _blobClientMock.Verify(x => x.UploadAsync(stream, It.IsAny<BlobUploadOptions>(), default), Times.Once);
            Assert.Equal(fakeUri.ToString(), result);
        }

        [Fact]
        public async Task Download_ShouldReturnStream_WhenFileExists_Async() {
            // Arrange
            string fileName = "dokument.pdf";
            string contentType = "application/pdf";
            var fileContent = new byte[] { 10, 20, 30 };
            using var memoryStream = new MemoryStream(fileContent);

            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(true, new Mock<Response>().Object));

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(
                contentType: contentType,
                contentLength: fileContent.Length,
                content: memoryStream
            );


            _blobClientMock
                .Setup(x => x.DownloadAsync(default))
                .ReturnsAsync(Response.FromValue(downloadInfo, new Mock<Response>().Object));

            // Act
            var result = await _service.DownloadAsync(fileName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contentType, result.Value.ContentType);

            using var resultMs = new MemoryStream();
            await result.Value.FileStream.CopyToAsync(resultMs);
            Assert.Equal(fileContent, resultMs.ToArray());
        }

        [Fact]
        public async Task Download_ShouldReturnNull_WhenFileDoesNotExist_Async() {
            // Arrange
            _blobClientMock
                .Setup(x => x.ExistsAsync(default))
                .ReturnsAsync(Response.FromValue(false, null!)); 

            // Act
            var result = await _service.DownloadAsync("nieistnieje.pdf");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteIfExists_Async() {
            // Arrange
            string fileName = "do_usuniecia.jpg";

            // Act
            await _service.DeleteAsync(fileName);

            // Assert
            _blobClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), null, default), Times.Once);
        }
    }
}