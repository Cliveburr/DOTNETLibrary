namespace Runner.WebUI.Components.Modal.Inputs
{
    public class InputFileResponse
    {
        public required List<InputFile> Files { get; set; }
    }

    public class InputFile
    {
        public required string Name { get; set; }
        public required byte[] Content { get; set; }
    }
}
