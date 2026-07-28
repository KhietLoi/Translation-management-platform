using Microsoft.AspNetCore.Mvc;

namespace MySolution.Api.Controllers;

public class TranslationKeyController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}