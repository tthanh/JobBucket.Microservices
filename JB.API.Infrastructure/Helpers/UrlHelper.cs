using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JB.Infrastructure.Helpers
{
    public static class UrlHelper
    {
        public static string GetBaseUrl(HttpContext httpContext)
        {
            return httpContext.Request.Scheme+ "://" + httpContext.Request.Host + "/";
        }
    }
}
