using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.Application;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SdkController : Controller
{
   //Test api:
   [HttpGet("translations")]
   [ApiKeyAuthorize]
   [ApiKeyPermission(ApiKeyPermissionType.TranslationRead)]
   public IActionResult GetTranslations()
   {
      return Ok(new { Message = "Success" });
   }
}