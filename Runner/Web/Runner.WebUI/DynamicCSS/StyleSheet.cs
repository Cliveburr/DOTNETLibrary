namespace Runner.WebUI.DynamicCSS
{
    public class StyleSheet
    {
        public string Id { get; set; }
        public readonly List<StyleClass> Classes;

        public StyleSheet(string id)
        {
            Id = id;
            Classes = new List<StyleClass>();
        }

        public void AddClass(StyleClass styleClass)
        {

        }
    }
}
