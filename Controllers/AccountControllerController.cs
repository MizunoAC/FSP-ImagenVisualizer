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

        return Ok(new
        {
            message = result.Message,
            error = false
        });
    }

}