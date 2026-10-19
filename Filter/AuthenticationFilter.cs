using FSP_Images.Models;
using FSP_Images.Services;
using FSP_Images.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FSP_Images.Filter;
public class AuthenticationFilter(TokenSessionServices tokenSession, JwtTokenService jwt) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var sessionId = context.HttpContext.Request.Cookies["sid"];
        var token = !string.IsNullOrEmpty(sessionId)  ? tokenSession.GetToken(sessionId) : null;

        if (token == null)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        var claims = token == null ? null : jwt.Validate(token.AccessToken);

        if (claims == null || !jwt.IsAdmin(claims))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        context.HttpContext.User = claims;
        context.HttpContext.Items["AccessToken"] = token!.AccessToken;
    }
}