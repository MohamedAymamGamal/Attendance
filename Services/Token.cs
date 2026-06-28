using Job.Models;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ess;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Job.Services
{
    public class Token 
    {
        private readonly IConfiguration _configuration;
        public Token(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GetToken( AppUsers user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim (ClaimTypes.Email,user.Email),
                new Claim (ClaimTypes.Name,user.UserName)
            };
            var Security = _configuration["Token:Secret"];
            var key = Encoding.ASCII.GetBytes(Security);

            SigningCredentials credentials = new 
                SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);



            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
               

                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Token:Issuer"],
                SigningCredentials = credentials,
                NotBefore = DateTime.Now,
                Audience = _configuration["Token:Audience"],
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);

        }
    }
}
