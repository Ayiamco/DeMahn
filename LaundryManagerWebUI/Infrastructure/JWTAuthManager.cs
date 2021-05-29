using LaundryManagerWebUI.Dtos;
using LaundryManagerWebUI.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerWebUI.Infrastructure
{
    public class JWTAuthManager : IJWTManager
    {
        private string _signingKey;
        public JWTAuthManager(IConfiguration config)
        {
            this._signingKey = config["authKey"];
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(JWTDto model)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //checks that the same client that recieved the token is the same client that wants to refresh.
                ValidateIssuer = false, //check that the twas the same system that issued the token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_signingKey)),
                ValidateLifetime = false // checks the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(model.JwtToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public string GetToken(JWTDto model)
        {
            //create security token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_signingKey);

            //create token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.PrimarySid, model.UserId),
                    new Claim(ClaimTypes.Role, model.UserRole),
                    new Claim(ClaimTypes.Email, model.UserEmail)
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

       
    }
}
