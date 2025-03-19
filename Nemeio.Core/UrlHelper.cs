using System;

namespace Nemeio.Core
{
    public static class UrlHelper
    {
        public static bool IsValidHttpUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException($"<{nameof(url)}> is null or empty");
            }

            Uri createdUri;

            var created = Uri.TryCreate(url, UriKind.Absolute, out createdUri);
            if (!created)
            {
                return false;
            }

            var isHttpScheme = createdUri.Scheme == Uri.UriSchemeHttp || createdUri.Scheme == Uri.UriSchemeHttps;

            return isHttpScheme;
        }
    }
}
