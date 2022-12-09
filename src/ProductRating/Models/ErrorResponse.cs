namespace ProductRating.Models;

public class ErrorResponse
{
    public List<Error> Errors { get; set; } = new();
}

public class Error
{
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}