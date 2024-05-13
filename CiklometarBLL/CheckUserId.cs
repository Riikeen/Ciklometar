using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CiklometarBLL
{
    public static class CheckUserId
    {
        public static Guid GetUserId(this HttpContext httpContext)
        {
            if(httpContext.User == null)
            {
                return Guid.Empty;
            }
            return Guid.Parse(httpContext.User.Claims.Single(x => x.Type == "id").Value);
        }
    }
}
