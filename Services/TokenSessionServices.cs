using FSP_Images.Models;
using Microsoft.Extensions.Caching.Memory;

namespace FSP_Images.Services;

public class TokenSessionServices(IMemoryCache memoryCache)
{
    public static readonly TimeSpan SessionDuration = TimeSpan.FromHours(1);

    public static string Key(string sessionId) => $"TokenSession_{sessionId}";
    public void SetToken(string sessionId, LoginResponse token)
    {
        memoryCache.Set(Key(sessionId), token, SessionDuration);
    }

    public LoginResponse GetToken(string sessionId) => memoryCache.TryGetValue(Key(sessionId), out LoginResponse token) ? token: null;

    public void RemoveToken(string sessionId) => memoryCache.Remove(Key(sessionId));
}
