using HtmlAgilityPack;

namespace FSP_Images.Services;

public record ImageFile(string Name, string Url);

public record FolderContent(List<string> Folders, List<ImageFile> Images);

/// <summary>
/// Reads the IIS directory listing of the remote folder and returns its subfolders and images.
/// </summary>
public class ImageService
{
    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly HttpClient _http;
    private readonly Uri _baseUrl;

    public ImageService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = new Uri(config["Images:BaseUrl"]!.TrimEnd('/') + "/");
    }

    /// <summary>Normalizes a relative path and rejects "." and "..".</summary>
    public static string CleanPath(string? path)
    {
        var parts = (path ?? "").Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Any(p => p == "." || p == ".."))
            throw new ArgumentException("Invalid path.");

        return string.Join("/", parts);
    }

    public async Task<FolderContent> GetContentAsync(string path)
    {
        var folderUri = BuildUri(path, isFolder: true);
        var html = await _http.GetStringAsync(folderUri);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var folderPath = Uri.UnescapeDataString(folderUri.AbsolutePath);
        var folders = new List<string>();
        var images = new List<ImageFile>();

        foreach (var link in doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
        {
            var href = HtmlEntity.DeEntitize(link.GetAttributeValue("href", ""));
            if (!Uri.TryCreate(folderUri, href, out var uri)) continue;

            var itemPath = Uri.UnescapeDataString(uri.AbsolutePath);
            if (uri.Host != folderUri.Host || itemPath.Length <= folderPath.Length ||
                !itemPath.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase)) continue;

            var name = itemPath.Substring(folderPath.Length);

            if (name.EndsWith('/'))
            {
                name = name.TrimEnd('/');
                if (!name.Contains('/')) folders.Add(name);
            }
            else if (!name.Contains('/') && ImageExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            {
                var imagePath = path == "" ? name : path + "/" + name;
                images.Add(new ImageFile(name, BuildUri(imagePath, isFolder: false).AbsoluteUri));
            }
        }

        return new FolderContent(
            folders.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList(),
            images.OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase).ToList());
    }

    private Uri BuildUri(string path, bool isFolder)
    {
        if (path == "") return _baseUrl;

        var escaped = string.Join("/", path.Split('/').Select(Uri.EscapeDataString));
        return new Uri(_baseUrl, escaped + (isFolder ? "/" : ""));
    }

    public Task<HttpResponseMessage> DownloadAsync(ImageFile image) =>
    _http.GetAsync(image.Url, HttpCompletionOption.ResponseHeadersRead);
}
