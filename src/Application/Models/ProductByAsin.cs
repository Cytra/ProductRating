namespace Application.Models;

public record ProductByAsin(
    string? Image,
    string? Price,
    string? Rating,
    int Reviews,
    string? Title
    );