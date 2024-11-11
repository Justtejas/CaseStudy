using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudyAPI.Controller
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController:ControllerBase
    {
        [HttpGet]
        public string GetData()
        {
            return "This is a String";
        }
    }
}
