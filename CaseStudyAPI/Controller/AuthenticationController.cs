using CaseStudyAPI.Authentication;
using CaseStudyAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public AuthenticationController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var tokenResponse = await _userServices.Login(model);
            if(tokenResponse == null)
            {
              return Unauthorized("Enter Valid Email or Password");
            }
            return Ok(new
            {
                token =  tokenResponse.Token,
                expiration = tokenResponse.Expiration
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var response = await _userServices.Register(model);
            return Ok(response);
        }

    }
}
