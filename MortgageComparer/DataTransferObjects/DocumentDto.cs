namespace MortgageComparer.DataTransferObjects;

public class DocumentDto {
    public string? DocumentKey { get; set; } 
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = "document.pdf";
    public string ContentType { get; set; } = "application/pdf";
}