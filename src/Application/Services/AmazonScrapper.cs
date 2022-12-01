using System.ComponentModel.Design;
using Application.Options;
using Application.Ports;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace Application.Services;

public interface IAmazonScrapper
{
    Task GetHotDeals();
}

public class AmazonScrapper : IAmazonScrapper
{
    private readonly IAmazonHttpClient _amazonHttpClient;
    public AmazonScrapper(
        IAmazonHttpClient amazonHttpClient)
    {
        _amazonHttpClient = amazonHttpClient;
    }

    public async Task GetHotDeals()
    {

        var oneDeal = await _amazonHttpClient.GetOneDeal();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(oneDeal);

        var paging = htmlDoc.DocumentNode
            .Descendants("div")
            .SingleOrDefault(node => node.GetAttributeValue("id", "")
                .Contains("page-navigation-container"));

        //div with id averageCustomerReviews

        var test = htmlDoc.GetElementbyId("averageCustomerReviews");

        var content = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='averageCustomerReviews']");
    }
}