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
            // abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890

            return Ok(passwordService.CrackPassword(
                "26775436073E00D207E192857EE3730CFCA19DE16F01F0780096EF217C2919EF",
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890",
                6)
                .Result);
        }
    }
}
