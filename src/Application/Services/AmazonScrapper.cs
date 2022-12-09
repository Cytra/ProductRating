using System.Runtime.CompilerServices;
using Application.Models;
using Application.Ports;
using HtmlAgilityPack;

namespace Application.Services;

public interface IAmazonScrapper
{
    Task<List<ProductRating>> GetProductsBySearchTerm(string searchTerm);

    Task GetProductByAsin(string asin);
}

public class AmazonScrapper : IAmazonScrapper
{
    private readonly IAmazonHttpClient _amazonHttpClient;
    public AmazonScrapper(
        IAmazonHttpClient amazonHttpClient)
    {
        _amazonHttpClient = amazonHttpClient;
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

        var oneProd = searchResult[0];

        var asin = oneProd.Attributes
            .Single(x => x.Name == "data-asin")
            .Value;
        var description = oneProd
            .Descendants("span")
            .Single(y => y.Attributes
                .Any( c => c.Name == "class" 
                           && c.Value == "a-size-medium a-color-base a-text-normal"))
            .InnerText;

        var priceText = oneProd
            .Descendants("span")
            .Single(y => y.Attributes
                .Any(c => c.Name == "class"
                          && c.Value == "a-offscreen"))
            .InnerText;



        var test2 = htmlDoc
            .DocumentNode
            .Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
                .Contains("a-section a-spacing-small a-spacing-top-small"))
            .ToList();

        var test3 = test2
            .Select(x => x.ChildNodes.First())
            .ToList();

        var test4 = test3
            .Select(x => x.ChildNodes.First())
            .ToList();

        var test5 = test4
            .Select(x => x.ChildNodes.First())
            //.Select(y=> y.Attributes.Where(a => a.Name == "href"))
            .ToList();

        var test6 = test5
            .Select(x=> x.ChildAttributes("href").First().Value)
            .ToList();



        //test5
        //    .Select(x=> x.Attributes.Where(y => y.Name == "href"));

        return new List<ProductRating>();
    }

    public async Task GetProductByAsin(string asin)
    {
        var productHtml = await _amazonHttpClient.GetProductByAsin(asin);
    }
}