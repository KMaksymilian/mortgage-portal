namespace MortgageComparer.Services.Interfaces;

public interface IJobTypeService
{
    public Task<JobTypeDocumentDto> GetJobAndDocumentAsync();
}