namespace FSP_Images.Models;

public class ApiErrorResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public bool? Error { get; set; }
}