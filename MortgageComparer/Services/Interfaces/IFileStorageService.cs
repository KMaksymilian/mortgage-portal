namespace MortgageComparer.Services.Interfaces {
    public interface IFileStorageService {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType = "application/pdf");
        Task<byte[]?> DownloadAsync(string fileUrlOrName);
        Task DeleteAsync(string fileUrlOrName);
    }
}