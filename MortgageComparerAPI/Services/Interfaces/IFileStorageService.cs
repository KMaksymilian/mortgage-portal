namespace MortgageComparerAPI.Services.Interfaces {
    public interface IFileStorageService {

        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType = "application/pdf");

        Task<(Stream? FileStream, string ContentType)?> DownloadAsync(string fileUrlOrName);

        Task DeleteAsync(string fileUrlOrName);
    }
}