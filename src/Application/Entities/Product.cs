namespace Application.Entities;

public class Product : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public float Rating { get; set; }
    public string? AmazonProductId { get; set; }

    //Images

}

public class ProductReview
{
    public DateTime Created { get; set; }
    public string? Summary { get; set; }
    public int Stars { get; set; }
    public string? Description { get; set; }

    //Images

}