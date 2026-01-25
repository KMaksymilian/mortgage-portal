namespace MortgageComparerAPI.Services.Interfaces {
    public interface IEmailTemplateService {
        // 1. Potwierdzenie złożenia wniosku (Dla Użytkownika)
        string GetSubmissionConfirmation(string userName, string bankName, decimal amount);

        // 2. Info dla pracownika banku (Nowy wniosek)
        string GetNewApplicationAlert(int applicationId, string userName, DateTime date);

        // 3. Wstępna akceptacja i dokumenty (Dla Użytkownika)
        string GetOfferAcceptedWithDocs(string userName, string signLink);

        // 4. Użytkownik podpisał dokumenty (Dla Admina)
        string GetSignedDocsAlert(int applicationId, string userName);

        // 5. Ostateczne potwierdzenie (Dla Użytkownika)
        string GetFinalApproval(string userName, decimal amount);

        // 6. Odrzucenie oferty (Dla Użytkownika)
        string GetRejection(string userName, string reason);
        string GetContractSigningReminder(string firstName);
    }
}