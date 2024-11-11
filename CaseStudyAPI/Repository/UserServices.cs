using CaseStudyAPI.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CaseStudyAPI.Repository
{
    public class UserServices : IUserServices
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public async Task<TokenResponse?> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new (ClaimTypes.Name, user.UserName),
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    expires: DateTime.Now.AddMinutes(20),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return new TokenResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                };
            }
            return null;
        }

        public async Task<Response> Register(RegisterModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.Username);
            if (userExist != null)
            {
                return new Response
                {
                    Status = "Error",
                    Message = "User already Exist!"
                };
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new Response
                {
                    Status = "Error",
                    Message = " User Creation Failed! Please check the user details and try again"
                };
            }
            if (model.Role.ToLower() == "jobseeker")
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.JobSeeker))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.JobSeeker));
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.JobSeeker))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.JobSeeker);
                }
            }
            if (model.Role.ToLower() == "employer")
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Employer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Employer));
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.Employer))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Employer);
                }
            }
            return new Response { Status = "Success", Message = "User Created Successfully" };
        }
    }
}
