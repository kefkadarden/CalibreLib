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
            Console.WriteLine($"Version: {version}");

            //Use FileVersion if version not found.
            if (version is null)
            {
                version =
                    $" FileV:{Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()
                    .Version}";
            }

            Console.WriteLine($"Version2: {version}");
            return $"v. {version}" ?? "UNKNOWN VERSION";
        }
    }
}
