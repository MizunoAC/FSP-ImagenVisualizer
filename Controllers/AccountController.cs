using FSP_Images.Models;
using FSP_Images.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace FSP_Images.Controllers;

public class AccountController(IAuthenticationService authenticationService) : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authenticationService.LoginAsync(request);

        if (result.Error == true)
        {
            return StatusCode(401, new
            {
                status = 401,
                message = result.Message,
                error = true
            });
        }

        Response.Cookies.Append("sid", result.SessionId, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromHours(8)
        });

        return Ok(new
        {
            message = result.Message,
            error = false
        });
    }

}