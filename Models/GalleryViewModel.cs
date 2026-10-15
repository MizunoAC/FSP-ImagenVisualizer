using FSP_Images.Services;

namespace FSP_Images.Models;

public class GalleryViewModel
{
    public string Path { get; set; } = "";
    public List<string> Folders { get; set; } = new();
    public List<ImageFile> Images { get; set; } = new();
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int StartIndex { get; set; }
    public string? Error { get; set; }
}
