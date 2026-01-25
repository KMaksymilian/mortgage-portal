using MortgageComparer.Entities;

namespace MortgageComparer.StatesMachine {

    public enum OfferStatus {
        Created = 0,        
        Pending = 1,        
        Approved = 2,      
        Rejected = 3, 
        ContractSigned = 4,
        Completed = 5,      
        Canceled = 6        
    }
    public interface IOfferStateMachine {
        public void Submit();
        public void Approve();        
        public void Reject(string reason);       
        public void Sign(string signedBy, string documentLink, DateTime contractValidDate);
        public void Complete();
        public void Cancel();
    }

    public class OfferStateMachine : IOfferStateMachine {
        private readonly OfferEntity _offer;

        public OfferStateMachine(OfferEntity offer) {
            _offer = offer ?? throw new ArgumentNullException(nameof(offer));
        }

        public void Submit() {
            EnsureStatus(OfferStatus.Created);
            _offer.Status = OfferStatus.Pending;
        }

        public void Approve() {
            EnsureStatus(OfferStatus.Pending);
            _offer.Status = OfferStatus.Approved;
        }

        public void Reject(string reason) {
            if (_offer.Status == OfferStatus.Completed) {
                throw new InvalidOperationException("Not allowed to reject completed offer.");
            }
            _offer.Status = OfferStatus.Rejected;
            _offer.StatusDescription = reason;
        }

        public void Sign(string signedBy, string documentLink, DateTime contractValidDate) {
            EnsureStatus(OfferStatus.Approved);
            _offer.Status = OfferStatus.ContractSigned;
            _offer.SingedBy = signedBy;
            _offer.DocumentLink = documentLink;
            _offer.ContractLinkValidDate = contractValidDate;
        }

        public void Complete() {
            EnsureStatus(OfferStatus.ContractSigned);
            _offer.Status = OfferStatus.Completed;
        }

        public void Cancel() {
            if (_offer.Status == OfferStatus.Completed) {
                throw new InvalidOperationException("Not allowed to cancel completed offer.");
            }
            _offer.Status = OfferStatus.Canceled;
        }

        private void EnsureStatus(OfferStatus requiredStatus) {
            if (_offer.Status != requiredStatus) {
                throw new InvalidOperationException($"Action not allowed. Current state: {_offer.Status}, required: {requiredStatus}");
            }
        }
    }

}
