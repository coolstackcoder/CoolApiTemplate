namespace OAuthCore.Application.DTOs;

public class ErrorResponseDto
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }

    public ErrorResponseDto(string type, string title, int status, string detail, string instance)
    {
        Type = type;
        Title = title;
        Status = status;
        Detail = detail;
        Instance = instance;
    }
}