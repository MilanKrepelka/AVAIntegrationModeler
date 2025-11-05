using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AVAIntegrationModeler.Web.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string redirectUri)
    {
        if (!string.IsNullOrEmpty(culture))
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture, culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }

        return LocalRedirect(redirectUri ?? "/");
    }
}
