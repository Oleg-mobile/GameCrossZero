namespace GameApp.Mvc.ViewModels
{
    public class ErrorViewModel  // TODO �����?
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}