using MortgageComparer.StatesMachine;

namespace MortgageComparer.StateMachine {
    public interface IOfferAction {
        string Name { get; }
        void Execute(IOfferStateMachine stateMachine);
    }

    public record SubmitAction : IOfferAction {
        public string Name => "Submit";
        public void Execute(IOfferStateMachine stateMachine) => stateMachine.Submit();
    }

    public record ApproveAction : IOfferAction {
        public string Name => "Approve";
        public void Execute(IOfferStateMachine stateMachine) => stateMachine.Approve();
    }

    public record RejectAction(string Reason) : IOfferAction {
        public string Name => "Reject";
        public void Execute(IOfferStateMachine stateMachine) => stateMachine.Reject(Reason);
    }

    public record SignAction(string SignedBy, string DocumentLink, DateTime ValidUntil) : IOfferAction {
        public string Name => "Sign";
        public void Execute(IOfferStateMachine stateMachine) =>
            stateMachine.Sign(SignedBy, DocumentLink, ValidUntil);
    }

    public record CompleteAction : IOfferAction {
        public string Name => "Complete";
        public void Execute(IOfferStateMachine stateMachine) => stateMachine.Complete();
    }

    public record CancelAction : IOfferAction {
        public string Name => "Cancel";
        public void Execute(IOfferStateMachine stateMachine) => stateMachine.Cancel();
    }

    public record ActionRequest(string Action, string? Reason, string? DocLink, DateTime? Expiry);
    public static class OfferActionFactory {
        public static IOfferAction Create(ActionRequest request) => request.Action.ToLower() switch {
            "submit" => new SubmitAction(),
            "approve" => new ApproveAction(),
            "reject" => new RejectAction(request.Reason ?? "No reason provided"),
            "sign" => new SignAction("System", request.DocLink ?? "", request.Expiry ?? DateTime.Now.AddDays(7)),
            "cancel" => new CancelAction(),
            "complete" => new CompleteAction(),
            _ => throw new ArgumentException($"Action '{request.Action}' is not supported.")
        };
    }
}
