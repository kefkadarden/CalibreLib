using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CalibreLib.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string AppVersion(this IHtmlHelper htmlHelper)
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyVersionAttribute>()
                ?.Version;

            //Use FileVersion if version not found.
            if (version is null)
            {
                version = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()
                    .Version;
            }

            return $"v. {version}" ?? "UNKNOWN VERSION";
        }
    }
}
