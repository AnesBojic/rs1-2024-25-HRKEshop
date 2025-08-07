
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RS1_2024_25.API.Services;
using RS1_2024_25.API.Services.Interfaces;

namespace RS1_2024_25.API.Endpoints.Auth1Endpoints
{
    [Route("test-email")]
    public class TestMailFast : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestMailFast(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> SendTestMail()
        {
            await _emailService.SendAsync(
                "ahmed.sic6382@gmail.com",
                "Cao brate jesi dobro",
                "<h1>Evo novi</h1><p>Kako je</p>"

                );

            return Ok("Test email sent");
        }


    }
}
