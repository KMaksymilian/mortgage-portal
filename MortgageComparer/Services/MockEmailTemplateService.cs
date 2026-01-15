using MortgageComparer.Services.Interfaces;
using System.Globalization;

namespace MortgageComparer.Services {
    public class MockEmailTemplateService : IEmailTemplateService {
        // --- 1. Potwierdzenie złożenia wniosku ---
        public string GetSubmissionConfirmation(string userName, string bankName, decimal amount) {
            var content = $@"
                <p>Cześć <strong>{userName}</strong>,</p>
                <p>Dziękujemy za wybranie oferty w Loan Hub. Twój wniosek został poprawnie zarejestrowany.</p>
                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                    <p style='margin: 5px 0;'><strong>Wybrany bank:</strong> {bankName}</p>
                    <p style='margin: 5px 0;'><strong>Kwota:</strong> {amount.ToString("C", new CultureInfo("pl-PL"))}</p>
                </div>
                <p>Wkrótce otrzymasz od nas kolejną wiadomość.</p>";

            return GetBaseHtml("Wniosek przyjęty! 🚀", content);
        }

        // --- 2. Info dla pracownika banku ---
        public string GetNewApplicationAlert(int applicationId, string userName, DateTime date) {
            var content = $@"
                <p>W systemie pojawił się nowy wniosek kredytowy.</p>
                <ul>
                    <li><strong>ID Wniosku:</strong> #{applicationId}</li>
                    <li><strong>Klient:</strong> {userName}</li>
                    <li><strong>Data:</strong> {date:yyyy-MM-dd HH:mm}</li>
                </ul>
                <p>Zaloguj się do panelu, aby zweryfikować klienta.</p>
                <a href='https://twoja-domena.com/admin' class='btn'>Przejdź do Panelu</a>";

            return GetBaseHtml("Nowy wniosek do rozpatrzenia ⚠️", content);
        }

        // --- 3. Wstępna akceptacja i dokumenty ---
        public string GetOfferAcceptedWithDocs(string userName, string signLink) {
            var content = $@"
                <p>Szanowny Panie / Szanowna Pani <strong>{userName}</strong>,</p>
                <p>Bank pozytywnie rozpatrzył Twój wniosek. Przesyłamy umowę do wglądu.</p>
                <p>Kliknij poniżej, aby zapoznać się z dokumentami i je podpisać.</p>
                <a href='{signLink}' class='btn'>Podpisz Umowę</a>
                <p style='margin-top: 20px; font-size: 12px; color: #666;'>Link jest ważny przez 10 dni.</p>";

            return GetBaseHtml("Twój wniosek został zaakceptowany ✅", content);
        }

        // --- 4. Klient podpisał dokumenty (Dla Admina) ---
        public string GetSignedDocsAlert(int applicationId, string userName) {
            var content = $@"
                <p>Użytkownik <strong>{userName}</strong> przesłał podpisane dokumenty do wniosku #{applicationId}.</p>
                <p>Wymagana jest ostateczna weryfikacja i uruchomienie środków.</p>";

            return GetBaseHtml("Klient podpisał umowę ✍️", content);
        }

        // --- 5. Ostateczne potwierdzenie (Kredyt uruchomiony) ---
        public string GetFinalApproval(string userName, decimal amount) {
            var content = $@"
                <p>Cześć <strong>{userName}</strong>,</p>
                <p>Weryfikacja zakończona sukcesem! Twoja pożyczka na kwotę <strong>{amount.ToString("C", new CultureInfo("pl-PL"))}</strong> została uruchomiona.</p>
                <p>Środki trafią na konto w najbliższym dniu roboczym.</p>";

            return GetBaseHtml("Gratulacje! Pożyczka przyznana 🎉", content);
        }

        // --- 6. Odrzucenie oferty ---
        public string GetRejection(string userName, string reason) {
            var content = $@"
                <p>Szanowny Panie / Szanowna Pani <strong>{userName}</strong>,</p>
                <p>Przykro nam, ale Twój wniosek nie mógł zostać zaakceptowany.</p>
                <div class='reason'>
                    <strong>Powód decyzji:</strong><br>
                    {reason}
                </div>
                <p>Zapraszamy do ponownego złożenia wniosku w przyszłości.</p>";

            return GetBaseHtml("Status Twojego wniosku", content);
        }

        // --- Metoda prywatna: Szablon Bazowy (Wrapper) ---

        private string GetBaseHtml(string title, string bodyContent) {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; padding: 20px; margin: 0; }}
                    .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); }}
                    .header {{ border-bottom: 2px solid #007bff; padding-bottom: 15px; margin-bottom: 25px; }}
                    .header h2 {{ margin: 0; color: #333; }}
                    .btn {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: #ffffff !important; text-decoration: none; border-radius: 4px; font-weight: bold; margin-top: 15px; }}
                    .footer {{ margin-top: 40px; font-size: 12px; color: #999; text-align: center; border-top: 1px solid #eee; padding-top: 15px; }}
                    .reason {{ background-color: #fff3cd; color: #856404; padding: 15px; border-left: 5px solid #ffeeba; margin: 15px 0; border-radius: 4px; }}
                    p {{ line-height: 1.6; color: #444; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>{title}</h2>
                    </div>
                    {bodyContent}
                    <div class='footer'>
                        Wiadomość wygenerowana automatycznie przez system Loan Hub.<br>
                        &copy; {DateTime.Now.Year} Loan Hub.
                    </div>
                </div>
            </body>
            </html>";
        }

        public string GetContractSigningReminder(string userName) {
            var content = $@"
        <p>Cześć <strong>{userName}</strong>,</p>
        <p>Zauważyliśmy, że Twoja umowa kredytowa została przygotowana 3 dni temu, ale wciąż nie została podpisana.</p>
        <div style='background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 15px 0; color: #856404;'>
            <strong>Uwaga:</strong> Masz łącznie 10 dni na podpisanie dokumentów. Po tym czasie oferta wygaśnie.
        </div>
        <p>Zaloguj się do panelu, aby dokończyć proces.</p>";

            return GetBaseHtml("Przypomnienie: Twoja umowa czeka! ⏳", content);
        }
    }
}
