using System.ComponentModel.Design;
using System.Xml.Linq;
using Application.Options;
using Application.Ports;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace Application.Services;

public interface IAmazonScrapper
{
    void GetProductsBySearchTerm(string searchTerm);
}

public class AmazonScrapper : IAmazonScrapper
{
    private readonly IAmazonHttpClient _amazonHttpClient;
    public AmazonScrapper(
        IAmazonHttpClient amazonHttpClient)
    {
        _amazonHttpClient = amazonHttpClient;
    }

    public void GetProductsBySearchTerm(string searchTerm)
    {
        var oneDeal = _amazonHttpClient.SearchProducts(searchTerm);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(oneDeal);


        //var allProductsTable = htmlDoc
        //    .DocumentNode
        //    .Descendants("div")
        //    .SingleOrDefault(node => node.GetAttributeValue("class", "")
        //        .Contains("s-main-slot s-result-list s-search-results sg-row"));

        //var allProducts = htmlDoc
        //    .DocumentNode
        //    .Descendants("div")
        //    .Where(node => node.GetAttributeValue("data-component-type", "")
        //        .Contains("s-search-result")).ToList();

        //// a class a-link-normal s-no-outline

        //var test = allProducts[0]
        //    .ChildNodes
        //    .Where(x => x.Name == "a" 
        //                //&& x.GetAttributeValue("class", "")
        //                //    .Contains("a-link-normal s-no-outline")
        //                );



        //var test2 = htmlDoc
        //    .DocumentNode
        //    .Descendants("div")
        //    .Where(node => node.GetAttributeValue("class", "")
        //        .Contains("a-section aok-relative s-image-fixed-height"))
        //    .ToList();

        //var test4 = test2
        //    .Select(x=> x.ChildNodes.First())
        //    .ToList();

        //s-product-image-container aok-relative s-image-overlay-grey s-text-center s-padding-left-small s-padding-right-small s-flex-expand-height

        var test2 = htmlDoc
            .DocumentNode
            .Descendants("div")
            .Where(node => node.GetAttributeValue("class", "")
                .Contains("s-product-image-container aok-relative s-image-overlay-grey s-text-center s-padding-left-small s-padding-right-small s-flex-expand-height"))
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

        var test7 = _amazonHttpClient.GetHtmlFromUrl(test6.First());

    }
}