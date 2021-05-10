using System;

namespace CivicCommunicator.Helpers
{
    public static class UrlHelper
    {
        public static bool IsUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
     
    }
}
