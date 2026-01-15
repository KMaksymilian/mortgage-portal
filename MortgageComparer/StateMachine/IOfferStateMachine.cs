using MortgageComparer.Entities;

namespace MortgageComparer.StatesMachine {

    public enum OfferStatus {
        Created = 0,        
        Pending = 1,        
        Approved = 2,      
        Rejected = 3, 
        ReadyToBeSigned = 4,
        ContractSigned = 5,
        Completed = 6,      
        Canceled = 7        
    }
    public interface IOfferStateMachine {
        public void Submit();
        public void Approve();        
        public void Reject(string reason);       
        public void Sign(string signedBy, string documentLink, DateTime contractValidDate);
        public void Complete();
    }

    public class OfferStateMachine : IOfferStateMachine {
        private readonly OfferEntity _offer;
        public OfferStateMachine(OfferEntity offer) {
            _offer = offer;
        }
        public void Submit() {
            _offer.Status = OfferStatus.Pending;
        }
        public void Approve() {
            _offer.Status = OfferStatus.Approved;
        }
        public void Reject(string reason) {
            _offer.Status = OfferStatus.Rejected;
            _offer.StatusDescription = reason;
        }
        public void Sign(string signedBy, string documentLink, DateTime contractValidDate) {
            _offer.Status = OfferStatus.ContractSigned;
            _offer.SingedBy = signedBy;
            _offer.DocumentLink = documentLink;
            _offer.ContractLinkValidDate = contractValidDate;
        }
        public void Complete() {
            _offer.Status = OfferStatus.Completed;
        }

        public void Cancel() {
            _offer.Status = OfferStatus.Canceled;
        }
    }
}
