namespace Application.Models;

public record ProductRating(
    string? Description,
    string Asin,
    string? Price,
    float? Rating,
    int? NumReviews,
    bool Sponsored
    );