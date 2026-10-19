using FSP_Images.Filter;
using FSP_Images.Services;
using FSP_Images.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ImageService>();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Backend:BaseUrl"]!);
});
builder.Services.AddScoped<AuthenticationFilter>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<TokenSessionServices>();
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
