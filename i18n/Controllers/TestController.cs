using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace i18n.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IStringLocalizer<TestController> _localizer;

        public TestController(ILogger<TestController> logger, IStringLocalizer<TestController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        [HttpGet(Name = "i18nTest")]
        public IActionResult TestLocalization()
        {
            // Fetch localized string
            string message = _localizer["shady"];
            return Ok(new { Message = message });
        }
    }
}
