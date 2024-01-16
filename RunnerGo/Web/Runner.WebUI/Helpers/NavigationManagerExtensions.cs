using Microsoft.AspNetCore.Components;

namespace Runner.WebUI.Helpers
{
    public static class NavigationManagerExtensions
    {
        public static bool MatchFullPath(this NavigationManager nav, params string[] path)
        {
            var navPath = nav.Uri
                .ToLower()
                .Substring(nav.BaseUri.Length)
                .Trim('/');
            var checkPath = path
                .Select(p => p.ToLower().Trim('/'))
                .ToList();
            return checkPath.Contains(navPath);
        }
    }
}
