using FSP_Images.Models;
using FSP_Images.Services.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSP_Images.Services;

public class AuthenticationService(HttpClient _http) : IAuthenticationService
{
    private const string LoginPath = "Authentication/Login";

    public async Task<LoginResponse> LoginAsync(LoginRequest login)
    {
        var body = new { username = login.Username, password =  login.Password };

        var response = await _http.PostAsJsonAsync(LoginPath, body);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>()
                ?? new LoginResponse
                {
                    Message = "Empty response from server."
                };
        }

        var error = await response.Content
            .ReadFromJsonAsync<ApiErrorResponse>();

        return new LoginResponse
        {
            Message = error?.Message ?? "Request failed.",
            Error = error?.Error,
            // Conserva también el status HTTP real si tu modelo lo contempla.
        };
    }
}