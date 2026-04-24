using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Telemachus.Business.Interfaces.Reports;

namespace Telemachus.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public DownloadController(IConfiguration config, IReportService reportService, IMemoryCache cache)
        {
            _reportService = reportService;
            _cache = cache;
            _config = config;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("sub", "temporary-token") }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store the token in memory cache with an expiration time
            _cache.Set(tokenString, "unused", TimeSpan.FromMinutes(5));

            return Ok(new { token = tokenString });
        }
        [HttpGet("{attachmentId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttachment([FromQuery(Name = "token")] string token, int attachmentId)
        {

            if (_cache.TryGetValue(token, out string tokenStatus) && tokenStatus == "unused")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]);

                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    _cache.Remove(token);

                    var attachment = await _reportService.GetAttachment(attachmentId);

                    var file = new FileInfo(attachment.FilePath);

                    if (!file.Exists)
                    {
                        return NotFound("File not found");
                    }

                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = file.Name,
                        Inline = true
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");

                    var stream = file.OpenRead();

                    return File(stream, attachment.MimeType);
                }
                catch
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet("waterConsumptions")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWaterConsumptions([FromQuery(Name = "token")] string token, [FromQuery(Name = "from")] DateTime? from, [FromQuery(Name = "to")] DateTime? to)
        {
            if (_cache.TryGetValue(token, out string tokenStatus) && tokenStatus != "unused")
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return Unauthorized();
            }

            _cache.Remove(token);

            if (!from.HasValue)
            {
                from = DateTime.MinValue.ToUniversalTime();
            }

            if (!to.HasValue)
            {
                to = DateTime.MaxValue.ToUniversalTime();
            }

            var fileInfo = await _reportService.GetWaterConsumptions(from.Value, to.Value);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileInfo.Name,
                Inline = true
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return File(fileInfo.OpenRead(), "application/pdf");
        }


    }
}
