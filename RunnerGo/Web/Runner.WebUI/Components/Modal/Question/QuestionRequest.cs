namespace Runner.WebUI.Components.Modal.Question
{
    public class QuestionRequest
    {
        public required string Title { get; set; }
        public required string Question { get; set; }
        public string OkText { get; set; } = "Ok";
        public string CloseText { get; set; } = "Close";
    }
}
