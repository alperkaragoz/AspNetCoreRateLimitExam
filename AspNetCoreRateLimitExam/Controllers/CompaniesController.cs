using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRateLimitExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        public IActionResult GetCompany()
        {
            return Ok(new { CompanyId = 1, CompanyName = "Google" });
        }
        [HttpPost]
        public IActionResult CreateCompany()
        {
            return Ok(new { StatusCode = 200 });
        }
    }
}
