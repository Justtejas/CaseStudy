using CaseStudyAPI.Data;
using CaseStudyAPI.Models;
using CaseStudyAPI.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CaseStudyAPI.Repository.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;
        private readonly SymmetricSecurityKey _authSigningKey;
        private readonly IConfiguration _configuration;
        public AuthorizationService(IConfiguration configuration)
        {
            _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));
            _configuration = configuration;
        }

        public TokenResponse GenerateJWTToken<T>(T user)
        {
            string userId = string.Empty;
            string userName = string.Empty;
            string role = string.Empty;

            if (user is JobSeeker jobSeeker)
            {
                userId = jobSeeker.JobSeekerId.ToString();
                role = "JobSeeker";
                userName = jobSeeker.UserName;
            }
            else if (user is Employer employer)
            {
                userId = employer.EmployerId.ToString();
                role = "Employer";
                userName = employer.UserName;
            }
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name,userName),
                new (ClaimTypes.NameIdentifier,userId),
                new (ClaimTypes.Role,role),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
