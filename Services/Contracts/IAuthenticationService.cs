using FSP_Images.Models;

namespace FSP_Images.Services.Contracts;

public interface IAuthenticationService
{
    Task<LoginResponse> LoginAsync(LoginRequest login);
}
