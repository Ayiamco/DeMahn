using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Infrastructure
{
    public static class ExtensionMethods
    {
        public static  string GetIdentityUserId(this  ControllerBase controllerBase)
        {
            var claims = controllerBase.User.Claims.ToList();
            var claimId = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            return claimId;

        }
    }
}
