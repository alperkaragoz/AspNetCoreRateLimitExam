using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreRateLimitExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCustomer()
        {
            return Ok(new { Id = 1, Name = "Alper" });
        }
        [HttpGet("{name}")]
        public IActionResult GetCustomer(string name)
        {
            return Ok(name);
        }

        [HttpPost]
        public IActionResult CreateCustomer()
        {
            return Ok(new { StatusCode = 200 });
        }
        [HttpPut]
        public IActionResult UpdateCustomer()
        {
            return Ok();
        }
    }
}
