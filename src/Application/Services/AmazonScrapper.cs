using Application.Models;
using Application.Ports;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAmazonScrapper
{
    Task<PagedList<ProductRating>> GetProductsBySearchTerm(string searchTerm, int? page);

    Task<Dictionary<string, ProductByAsin>> GetProductsByAsin(string[] asins);

    Task<ProductByAsin?> GetProductByAsin(string asins);
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

        if (searchResult == null || searchResult.Count == 0)
        {
            return new PagedList<ProductRating>();
        }

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
            TotalPages = canParseLastPageString ? lastPage : 1,
        };
    }

    private ProductRating ParseProduct(HtmlNode oneProd)
    {
        string? description = null;
        string? price = null;
        var asin = oneProd.Attributes
            .Single(x => x.Name == "data-asin")
            .Value;

        _logger.LogInformation("asin {asin}", asin);

        var descriptionNode = oneProd
            .Descendants("span")
            .FirstOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value.Contains("a-color-base a-text-normal")));

        if (descriptionNode != null)
        {
            description = descriptionNode.InnerText;
        }

        var priceNode = oneProd
            .Descendants("span")
            .FirstOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-price"));

        if (priceNode != null)
        {
            price = priceNode.FirstChild?.InnerText;
        }

        var ratingString = oneProd
            .Descendants("div")
            .SingleOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-row a-size-small"))
            ?.FirstChild?.InnerText;

        var canParseRating = float.TryParse(ratingString?[..3], out var rating);


        var numOfReviewsString = oneProd
            .Descendants("span")
            .FirstOrDefault(node => node.GetAttributeValue("class", "")
                .Contains("a-size-base s-underline-text"))
            ?.InnerText;

        numOfReviewsString = numOfReviewsString?
            .Trim(' ', '(', ')')
            .Replace(",", "");

        var carParseNumOfReviews = int.TryParse(
            numOfReviewsString, out int numOfReviews);

        var sponsoredSpan = oneProd
            .Descendants("span")
            .FirstOrDefault(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-color-secondary"));

        var sponsored = sponsoredSpan != null;

        return new ProductRating(
            description, 
            asin, 
            price,
            canParseRating ? rating : null,
            carParseNumOfReviews ? numOfReviews : null, 
            sponsored);
    }

    public async Task<Dictionary<string, ProductByAsin>> GetProductsByAsin(string[] asins)
    {
        var result = new Dictionary<string, ProductByAsin>();
        foreach (var asin in asins)
        {
            var product = await GetProductByAsin(asin);
            result.Add(asin, product);
        }
        return result;
    }

    public async  Task<ProductByAsin?> GetProductByAsin(string asin)
    {
        var productHtml = await _amazonHttpClient.GetProductByAsin(asin);
        var product = ParseProductByAsin(productHtml);
        return product;
    }

    private ProductByAsin ParseProductByAsin(string input)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(input);

        string? image = null;
        string? title = null;
        string? price = null;
        float? rating = 0;
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
            .FirstOrDefault(node => node.GetAttributeValue("class", "")
                .Contains("a-price aok-align-center reinventPricePriceToPayMargin priceToPay"));


        if (priceNode is { FirstChild: { } })
        {
            price = priceNode.FirstChild.InnerText;
        }
        else
        {
            var priceLowerRangeNode = htmlDoc
                .DocumentNode
                .Descendants("span")
                .FirstOrDefault(node => node.GetAttributeValue("class", "")
                    .Contains("a-price a-text-price a-size-medium apexPriceToPay"));

            if (priceLowerRangeNode != null)
            {
                price = priceLowerRangeNode.FirstChild.InnerText;
            }

        }

        var ratingNode = htmlDoc
            .DocumentNode
            .Descendants("span")
            .FirstOrDefault(node => node.GetAttributeValue("class", "")
                .Contains("a-size-medium a-color-base"));

        if (ratingNode != null)
        {
            var ratingString = ratingNode.FirstChild.InnerText;
            var canParseRating = float.TryParse(ratingString?[..3], out var ratingPared);
            if (canParseRating)
            {
                rating = ratingPared;
            }
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
            var reviewsStringTrimmed = reviewsStringSplit[0].Replace(",", "");
            var varParseReviewsCount = int.TryParse(reviewsStringTrimmed, out reviews);
        }

        return new ProductByAsin(
            image, 
            price,
            rating, 
            reviews, 
            title);
    }
}