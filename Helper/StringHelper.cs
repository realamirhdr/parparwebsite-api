using System.Text.RegularExpressions;

namespace ParParWebsite.Api.Helper
{
    public static class StringHelper
    {
        public static string ToSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1) Trim, lowercase
            string slug = input.Trim().ToLowerInvariant();

            // 2) Replace any sequence of whitespace with a single hyphen
            slug = Regex.Replace(slug, @"\s+", "-");

            // 3) Remove any character that is not a-z, 0-9 or hyphen
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // 4) Collapse multiple hyphens into one (in case step 3 left duplicates)
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }
    }
}
