namespace MortgageComparer.Models;

public class ContractDataDto
{
    public byte[] FileContents { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}