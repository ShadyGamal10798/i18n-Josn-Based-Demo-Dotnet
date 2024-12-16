using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;


namespace i18n.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IStringLocalizer<TestController> _localizer;
        private readonly IConfiguration _configuration;

        public TestController(ILogger<TestController> logger, IStringLocalizer<TestController> localizer, IConfiguration configuration)
        {
            _logger = logger;
            _localizer = localizer;
            _configuration = configuration;
        }

        [HttpGet(Name = "i18nTest")]
        public IActionResult TestLocalization()
        {
            // Fetch localized string
            string message = _localizer["shady"];
            return Ok(new { Message = message });
        }




        // P/Invoke for connecting to network shares
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(ref NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);

        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        private const int RESOURCETYPE_DISK = 0x1;

        // Network credentials and path
        private readonly string _networkPath = @"\\10.180.3.20\RedfDocs";
        private readonly string _username = @"nfsc\InternalApplication";
        private readonly string _password = "v2249PzG7zOO9mObd336";

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] TestTest model)
        {
            // Check if a file is provided
            if (model?.file == null || model.file.Length == 0)
            {
                return BadRequest("No file uploaded or file is empty.");
            }

            try
            {
                // Connect to the network share
                NETRESOURCE netResource = new NETRESOURCE
                {
                    dwType = RESOURCETYPE_DISK,
                    lpRemoteName = _networkPath
                };

                int result = WNetAddConnection2(ref netResource, _password, _username, 0);
                if (result != 0)
                {
                    throw new Exception($"Error connecting to network share: {result}");
                }

                // Save the uploaded file to the network path
                string fileName = Path.GetFileName(model.file.FileName);
                string destinationPath = Path.Combine(_networkPath, fileName);

                using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    await model.file.CopyToAsync(fileStream);
                }

                // Disconnect the network share
                WNetCancelConnection2(_networkPath, 0, true);

                return Ok(new { Message = $"File '{fileName}' uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Error uploading file: {ex.Message}" });
            }
        }




    }
}


public class TestTest
{
    public IFormFile file { get; set; }
}
