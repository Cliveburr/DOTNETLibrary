namespace Runner.WebUI.DynamicCSS
{
    public class Style
    {
        public string? BackgroundColor { get; set; }

        public override string ToString()
        {
            var values = new List<string>();

            if (BackgroundColor != null)
            {
                values.Add($"background-color: {BackgroundColor};");
            }

            return $"{{{string.Join(',', values)}}}";
        }
    }
}
