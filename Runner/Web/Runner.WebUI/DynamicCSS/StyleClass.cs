using System.Text.RegularExpressions;

namespace Runner.WebUI.DynamicCSS
{
    public class StyleClass : Style
    {
        public static int GeneratedSelectorCount = 0;

        public string Selector { get; set; }
        public string Name { get; set; }

        public StyleClass(string? selector = null, string? name = null)
        {
            Selector = selector ?? GenerateAutoSelector();
            Name = name ?? Selector;
        }

        public static string GenerateAutoSelector()
        {
            return "TT";

            //GeneratedSelectorCount++;
        }
    }
}
