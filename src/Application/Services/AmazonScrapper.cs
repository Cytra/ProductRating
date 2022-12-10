﻿using System.Runtime.CompilerServices;
using Application.Models;
using Application.Ports;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IAmazonScrapper
{
    Task<List<ProductRating>> GetProductsBySearchTerm(string searchTerm);

    Task GetProductByAsin(string asin);
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

    public async Task<List<ProductRating>> GetProductsBySearchTerm(string searchTerm)
    {
        var result = new List<ProductRating>();
        var oneDeal = await _amazonHttpClient.SearchProducts(searchTerm);

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

        return result;
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

    public async Task GetProductByAsin(string asin)
    {
        var productHtml = await _amazonHttpClient.GetProductByAsin(asin);
    }
}