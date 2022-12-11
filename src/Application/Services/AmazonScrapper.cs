using Application.Models;
using Application.Ports;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAmazonScrapper
{
    Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page);

    Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string[] asins);
}

public class AmazonScrapper : IAmazonScrapper
{
    private readonly IAmazonHttpClient _amazonHttpClient;
    private readonly ILogger<AmazonScrapper> _logger;
    public AmazonScrapper(
        IAmazonHttpClient amazonHttpClient, 
        ILogger<AmazonScrapper> logger)
    {
        _amazonHttpClient = amazonHttpClient;
        _logger = logger;
    }

    public async Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page)
    {
        var result = new List<ProductRating>();
        var realPage = page ?? 1;
        var oneDeal = await _amazonHttpClient.SearchProducts(searchTerm, realPage);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(oneDeal);

        var searchResultTable = htmlDoc
            .DocumentNode
            .Descendants("div")
            .Single(node => node.GetAttributeValue("class", "")
                .Contains("s-main-slot s-result-list s-search-results sg-row"));

        var searchResult = searchResultTable.ChildNodes
            .Where(x => x.Attributes.Any(y
                => y.Name == "data-component-type" && y.Value == "s-search-result"))
            .ToList();

        foreach (var node in searchResult)
        {
            result.Add(ParseProduct(node));
        }

        var paging = GetPaging(htmlDoc, realPage, result.Count);

        return new PagedList<ProductRating>()
        {
            Items = result,
            Paging = paging
        };
    }

    private PagingModel GetPaging(HtmlDocument document, int page, int size)
    {
        var lastPageString = document
            .DocumentNode
            .Descendants("span")
            .Single(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "s-pagination-item s-pagination-disabled"))
            .InnerText;

        var canParseLastPageString = int.TryParse(lastPageString, out var lastPage);

        return new PagingModel()
        {
            Page = page,
            PageSize = size,
            TotalPages = lastPage
        };
    }

    private ProductRating ParseProduct(HtmlNode oneProd)
    {
        
        var asin = oneProd.Attributes
            .Single(x => x.Name == "data-asin")
            .Value;

        _logger.LogInformation("asin {asin}", asin);

        var description = oneProd
            .Descendants("span")
            .Single(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-size-medium a-color-base a-text-normal"))
            .InnerText;

        var priceText = oneProd
            .Descendants("span")
            .SingleOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-price"))
            ?.FirstChild?.InnerText;

        var ratingString = oneProd
            .Descendants("div")
            .SingleOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-row a-size-small"))
            ?.FirstChild?.InnerText;

        var canParseRating = float.TryParse(ratingString?[..3], out var rating);

        var numOfReviewsString = oneProd
            .Descendants("span")
            .SingleOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-size-base s-underline-text"))
            ?.InnerText;

        numOfReviewsString = numOfReviewsString?.Trim(' ', '(', ')');

        var carParseNumOfReviews = int.TryParse(
            numOfReviewsString, out int numOfReviews);

        var sponsoredSpan = oneProd
            .Descendants("span")
            .SingleOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-color-secondary"));
        var sponsored = sponsoredSpan != null;

        return new ProductRating(
            description, 
            asin, 
            priceText,
            canParseRating ? rating : null,
            carParseNumOfReviews ? numOfReviews : null, 
            sponsored);
    }

    public async Task<Dictionary<string, ProductByAsin>> GetProductByAsin(string[] asins)
    {
        var result = new Dictionary<string, ProductByAsin>();
        foreach (var asin in asins)
        {
            var productHtml = await _amazonHttpClient.GetProductByAsin(asin);
            var product = ParseProductByAsin(productHtml);
            result.Add(asin, product);
        }
        return result;
    }

    private ProductByAsin ParseProductByAsin(string input)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(input);

        string? image = null;
        string? title = null;
        string? price = null;
        string? rating = null;
        var reviews = 0;

        var imageNodeDiv = htmlDoc
            .DocumentNode
            .Descendants("div")
            .FirstOrDefault(node => node.GetAttributeValue("id", "")
                .Contains("imgTagWrapperId"));

        var imageNodeImg = imageNodeDiv?
            .Descendants("img")
            .FirstOrDefault();

        image = imageNodeImg?.Attributes
                .FirstOrDefault(x => x.Name == "src")?.Value;

        var titleNode = htmlDoc
            .DocumentNode
            .Descendants("span")
            .FirstOrDefault(node => node.GetAttributeValue("id", "")
                .Contains("productTitle"));

        title = titleNode?.InnerText.Trim();

        var priceNode = htmlDoc
            .DocumentNode
            .Descendants("span")
            .SingleOrDefault(node => node.GetAttributeValue("class", "")
                .Contains("a-price aok-align-center reinventPricePriceToPayMargin priceToPay"));

        if (priceNode != null)
        {
            price = priceNode.FirstChild.InnerText;
        }

        var ratingNode = htmlDoc
            .DocumentNode
            .Descendants("span")
            .SingleOrDefault(node => node.GetAttributeValue("class", "")
                .Contains("a-size-medium a-color-base"));

        if (ratingNode != null)
        {
            rating = ratingNode.FirstChild.InnerText;
        }

        var reviewsNode = htmlDoc
            .DocumentNode
            .Descendants("span")
            .FirstOrDefault(node => node.GetAttributeValue("id", "")
                .Contains("acrCustomerReviewText"));

        if (reviewsNode != null)
        {
            var reviewsString = reviewsNode.InnerText;
            var reviewsStringSplit = reviewsString.Split(' ');
            var varParseReviewsCount = int.TryParse(reviewsStringSplit[0], out reviews);

        }

        return new ProductByAsin(
            image, 
            price, 
            rating, 
            reviews, 
            title);
    }
}