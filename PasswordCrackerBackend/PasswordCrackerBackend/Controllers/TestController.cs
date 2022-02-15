using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PasswordCrackerBackend.Services;

namespace PasswordCrackerBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly PasswordService passwordService;

        public TestController(PasswordService passwordService)
        {
            this.passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Crack()
        {
            // "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69";
            // "3086E346353248775A2C5D74E36A9C9B9BD226A1EE401F830AC499633DC00031";
            // "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF";
            // "43C19A093B34B581DDCC7207F6BD094F6940DB69F035C444425ED84D2CAC037D";

            // abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890
            var start = DateTime.Now;
            var password = passwordService.CrackPassword(
                    "A746222F09D85605C52D4E636788D6FFDC274698B98B8C5F3244C06958683A69",
                    "abcdefghijklmnopqrstuvwxyz",
                    4);
            var end = DateTime.Now;
            return Ok(new
            {
                password, 
                TimeSpan = end.Subtract(start),
            });
        }
    }
}
