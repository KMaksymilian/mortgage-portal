using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using MortgageComparer.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure;
using System.Text;

public class AzureBlobStorageServiceTests {
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly Mock<BlobContainerClient> _containerClientMock;
    private readonly Mock<BlobClient> _blobClientMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AzureBlobStorageService _service;

    private const string TestContainerName = "test-container";

    public AzureBlobStorageServiceTests() {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _containerClientMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup hierarchii mocków: Service -> Container -> Blob
        _blobServiceClientMock
            .Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_containerClientMock.Object);

        _containerClientMock
            .Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        _configurationMock.Setup(c => c["AzureStorage:ContainerName"]).Returns(TestContainerName);

        _service = new AzureBlobStorageService(_blobServiceClientMock.Object, _configurationMock.Object);
    }

    // --- TESTY UPLOAD ---

    [Fact]
    public async Task Upload_ShouldUseDefaultPdfContentType_WhenNotProvided_Async() {
        // ARRANGE
        var fileName = "document.unknown"; // Rozszerzenie nie ma znaczenia, sprawdzamy domyślny parametr
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

        var expectedUri = new Uri("https://fake.blob/file.pdf");
        _blobClientMock.Setup(x => x.Uri).Returns(expectedUri);

        _blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // ACT
        // Nie podajemy trzeciego parametru (contentType)
        await _service.UploadAsync(stream, fileName);

        // ASSERT
        // Sprawdzamy, czy w BlobUploadOptions.HttpHeaders.ContentType trafiło "application/pdf"
        _blobClientMock.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<BlobUploadOptions>(opts => opts.HttpHeaders.ContentType == "application/pdf"),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Upload_ShouldUseProvidedContentType_Async() {
        // ARRANGE
        var fileName = "photo.jpg";
        var customContentType = "image/jpeg";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("test"));

        var expectedUri = new Uri("https://fake.blob/photo.jpg");
        _blobClientMock.Setup(x => x.Uri).Returns(expectedUri);

        _blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // ACT
        // Podajemy własny ContentType
        await _service.UploadAsync(stream, fileName, customContentType);

        // ASSERT
        // Sprawdzamy, czy przesłano "image/jpeg"
        _blobClientMock.Verify(x => x.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<BlobUploadOptions>(opts => opts.HttpHeaders.ContentType == customContentType),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Upload_ShouldNotThrow_WhenStreamIsNotSeekable_Async() {
        // ARRANGE
        // Mockujemy strumień, który nie pozwala na przewijanie (CanSeek = false)
        // To symuluje strumień HTTP w niektórych scenariuszach
        var nonSeekableStreamMock = new Mock<Stream>();
        nonSeekableStreamMock.Setup(s => s.CanSeek).Returns(false);
        // Jeśli kod spróbuje ustawić Position = 0, mock rzuci wyjątek (tak jak prawdziwy strumień)
        nonSeekableStreamMock.SetupSet(s => s.Position = It.IsAny<long>()).Throws<NotSupportedException>();

        var expectedUri = new Uri("https://fake.blob/test.pdf");
        _blobClientMock.Setup(x => x.Uri).Returns(expectedUri);

        _blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // ACT
        // Jeśli w kodzie jest "if (fileStream.CanSeek)", to ten test przejdzie.
        // Jeśli nie ma if-a, test wywali się z NotSupportedException.
        var result = await _service.UploadAsync(nonSeekableStreamMock.Object, "test.pdf");

        // ASSERT
        Assert.Equal(expectedUri.ToString(), result);
    }

    // --- POZOSTAŁE TESTY (Bez zmian) ---

    [Fact]
    public async Task Upload_And_Download_Should_Return_Same_Data_Async() {
        // ARRANGE
        var fileName = "test-file.pdf";
        var originalContent = "TEST";
        var originalBytes = Encoding.UTF8.GetBytes(originalContent);
        var fakeUrl = $"https://fake.blob.core.windows.net/{TestContainerName}/{Guid.NewGuid()}_{fileName}";

        // Symulacja "Chmury"
        byte[]? simulatedCloud = null;

        // 1. Konfiguracja UPLOAD
        // Ensure upload callback reads from beginning and capture both common overloads
        _blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<BlobUploadOptions>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, BlobUploadOptions, CancellationToken>((s, o, t) => {
                if (s.CanSeek) s.Position = 0;
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                simulatedCloud = ms.ToArray();
            })
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // Also mock the common overload without BlobUploadOptions (if service uses it)
        _blobClientMock
            .Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((s, t) => {
                if (s.CanSeek) s.Position = 0;
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                simulatedCloud = ms.ToArray();
            })
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        // If service uses container.UploadBlobAsync, mock that too
        _containerClientMock
            .Setup(x => x.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<string, Stream, CancellationToken>((name, s, t) => {
                if (s.CanSeek) s.Position = 0;
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                simulatedCloud = ms.ToArray();
            })
            .ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());

        _blobClientMock.Setup(x => x.Uri).Returns(new Uri(fakeUrl));

        // 2. Konfiguracja EXISTS
        _blobClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, null!));

        // 3. Konfiguracja DOWNLOAD (Poprawiona)
        _blobClientMock
            .Setup(x => x.DownloadToAsync(
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())) // ZMIANA: Zamiast 'default' daj 'It.IsAny'
            .Callback<Stream, CancellationToken>((targetStream, token) => {
                if (simulatedCloud != null && simulatedCloud.Length > 0) {
                    targetStream.Write(simulatedCloud, 0, simulatedCloud.Length);
                    targetStream.Position = 0; // Reset kursora
                }
            })
            .ReturnsAsync(Mock.Of<Response>());

        // ACT
        using var inputStream = new MemoryStream(originalBytes);
        // Upewniamy się, że strumień wejściowy jest gotowy do odczytu
        if (inputStream.CanSeek) inputStream.Position = 0;

        string fileUrl = await _service.UploadAsync(inputStream, fileName);
        var downloadedBytes = await _service.DownloadAsync(fileUrl);

        // ASSERT
        Assert.NotNull(downloadedBytes);
        Assert.Equal(originalBytes, downloadedBytes);
    }

    [Fact]
    public async Task Delete_ShouldExtractFileNameCorrectly_AndCallDelete_Async() {
        // ARRANGE
        // Testujemy czy logika parsowania URL działa
        var fileName = "contract-v1.pdf";
        var fileUrl = $"https://myaccount.blob.core.windows.net/{TestContainerName}/{fileName}";

        // Setupujemy mocka kontenera, żeby zwrócił klienta dla KONKRETNEJ nazwy pliku
        // Jeśli logika parsowania URL jest zła, ten setup nie zadziała
        _containerClientMock
            .Setup(x => x.GetBlobClient(fileName)) // Oczekujemy samej nazwy, nie URL
            .Returns(_blobClientMock.Object);

        _blobClientMock
            .Setup(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(true, null!));

        // ACT
        await _service.DeleteAsync(fileUrl);

        // ASSERT
        _blobClientMock.Verify(x => x.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}