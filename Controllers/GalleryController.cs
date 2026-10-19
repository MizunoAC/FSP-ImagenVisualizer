using FSP_Images.Filter;
using FSP_Images.Models;
using FSP_Images.Services;
using Microsoft.AspNetCore.Mvc;

namespace FSP_Images.Controllers;

[ServiceFilter(typeof(AuthenticationFilter))]
public class GalleryController : Controller
{
    private readonly ImageService _images;
    private readonly int _pageSize;

    public GalleryController(ImageService images, IConfiguration config)
    {
        _images = images;
        _pageSize = config.GetValue("Images:PageSize", 24);
    }

    public async Task<IActionResult> Index(string? path, int page = 1)
    {
        string cleanPath;
        try { cleanPath = ImageService.CleanPath(path); }
        catch (ArgumentException) { return BadRequest(); }

        var model = new GalleryViewModel { Path = cleanPath };

        try
        {
            var content = await _images.GetContentAsync(cleanPath);

            model.Folders = content.Folders;
            model.TotalPages = Math.Max(1, (int)Math.Ceiling(content.Images.Count / (double)_pageSize));
            model.Page = Math.Clamp(page, 1, model.TotalPages);
            model.Images = content.Images.Skip((model.Page - 1) * _pageSize).Take(_pageSize).ToList();
            model.StartIndex = (model.Page - 1) * _pageSize;
        }
        catch (HttpRequestException)
        {
            model.Error = "No se pudo leer la carpeta remota. Revisa Images:BaseUrl en appsettings.json " +
                          "y que el directory browsing siga activo.";
        }

        return View(model);
    }

    // Serves the image through this app so the browser sees "fototrampaN.ext" instead of the original name.
    // n is the global 1-based position of the image in the folder (same numbering as the gallery).
    public async Task<IActionResult> Photo(string? path, int n, bool download = false)
    {
        string cleanPath;
        try { cleanPath = ImageService.CleanPath(path); }
        catch (ArgumentException) { return BadRequest(); }

        FolderContent content;
        try { content = await _images.GetContentAsync(cleanPath); }
        catch (HttpRequestException) { return NotFound(); }

        if (n < 1 || n > content.Images.Count) return NotFound();
        var image = content.Images[n - 1];

        var upstream = await _images.DownloadAsync(image);
        if (!upstream.IsSuccessStatusCode) return NotFound();
        Response.RegisterForDispose(upstream);

        var fileName = $"fototrampa{n}{System.IO.Path.GetExtension(image.Name).ToLowerInvariant()}";
        var contentType = upstream.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var stream = await upstream.Content.ReadAsStreamAsync();

        Response.Headers.ContentDisposition = $"{(download ? "attachment" : "inline")}; filename=\"{fileName}\"";
        return File(stream, contentType);
    }
}
