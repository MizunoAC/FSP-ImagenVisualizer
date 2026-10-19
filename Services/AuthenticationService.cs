using FSP_Images.Models;
using FSP_Images.Services.Contracts;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FSP_Images.Services;

public class AuthenticationService(HttpClient _http, TokenSessionServices tokenSession, JwtTokenService jwt) : IAuthenticationService
{
    private const string LoginPath = "Authentication/Login";

    public async Task<LoginResponse> LoginAsync(LoginRequest login)
    {
        var body = new { username = login.Username, password = login.Password };

        try
        {
            var response = await _http.PostAsJsonAsync(LoginPath, body);

            if (!response.IsSuccessStatusCode)
                return await ReadErrorAsync(response);

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            var claims = jwt.Validate(result.AccessToken);

            if (claims == null)
                return new LoginResponse { Message = "Respuesta inválida del servidor.", Error = true };

            if (!jwt.IsAdmin(claims))
                return new LoginResponse { Message = "Tu cuenta no tiene permiso para ver la galería.", Error = true };

            result.SessionId = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            tokenSession.SetToken(result.SessionId, result);

            var check = tokenSession.GetToken(result.SessionId);

            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
                                      or JsonException or NotSupportedException)
        {
            return new LoginResponse { Message = "No se pudo conectar con el servidor." };
        }
    }

    private static async Task<LoginResponse> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();

            return new LoginResponse
            {
                Message = error?.Message ?? "No se pudo iniciar sesión.",
                Error = error.Error
            };
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return new LoginResponse { Message = "No se pudo iniciar sesión." };
        }
    }
}