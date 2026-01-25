using MortgageComparer.Entities;

namespace MortgageComparer.Services.Interfaces;

public interface IExternalApiService
{
        Task<string?> GetTokenAsync();
        Task<JobTypeEntity> GetJobTypesAsync();
        Task<PersonalDocumentTypeEntity> GetDocumentTypesAsync();
}