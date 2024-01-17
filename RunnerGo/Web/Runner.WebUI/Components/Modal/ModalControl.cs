namespace Runner.WebUI.Components.Modal
{
    public class ModalControl
    {
        public object? Request { get; set; }
        public object? Response { get; set; }
        public required ManualResetEvent Resume { get; set; }
    }
}
