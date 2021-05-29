using LaundryManagerWebUI.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace LaundryManagerWebUI.Interfaces
{
    public interface IJWTManager
    {
        public string GetToken(JWTDto model);
        public ClaimsPrincipal GetPrincipalFromExpiredToken(JWTDto model);
        public string RefreshJwt(JWTDto model);
    }
}
