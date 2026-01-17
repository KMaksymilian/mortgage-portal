using MortgageComparer.Models;

namespace MortgageComparer.Services.Interfaces;

public interface IJobTypeService
{
    public Task<GetJobAndDocumentResponse> GetJobAndDocumentAsync();
}