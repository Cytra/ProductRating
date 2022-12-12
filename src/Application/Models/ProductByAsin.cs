namespace Application.Models;

public record ProductByAsin(
    string? Image,
    string? Price,
    float? Rating,
    int Reviews,
    string? Title
    );