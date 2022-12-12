using System.Reflection;
using Application.Ports;
using Application.Services;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests;

public class AmazonScrapperTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IAmazonHttpClient> _amazonHttpClientMock;

    public AmazonScrapperTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _amazonHttpClientMock = _fixture.Freeze<Mock<IAmazonHttpClient>>();
    }

    [Theory]
    [MemberData(nameof(GetProductByAsinData))]
    public async Task GetProductByAsin(
        string asin,
        string price,
        float rating,
        int reviews,
        string title,
        string image)
    {
        var rootFolder = "Data";
        var subFolder = "AsinData";
        var path = Path.Combine(rootFolder, subFolder, $"{asin}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);

        _amazonHttpClientMock.Setup(x => x.GetProductByAsin(It.IsAny<string>()))
            .ReturnsAsync(rawHtml);

        var sut = _fixture.Create<AmazonScrapper>();

        var products = await sut.GetProductByAsin(asin);

        products.Should().NotBeNull();
        var product = products[asin];
        product.Image.Should().Be(image);
        product.Price.Should().Be(price);
        product.Rating.Should().Be(rating);
        product.Reviews.Should().Be(reviews);
        product.Title.Should().Be(title);
    }

    public static IEnumerable<object[]> GetProductByAsinData =>
        new List<object[]>
        {
            new object[]
            {
                "B0714BNRWC",
                "$15.70",
                4.5f,
                115296,
                "adidas Men&#39;s Adilette Shower Slide",
                "https://m.media-amazon.com/images/I/51bSvh09-aL.__AC_SY395_SX395_QL70_ML2_.jpg"
            },
            new object[]
            {
                "B07TKNPPDL",
                "$17.95",
                4.6f,
                2741,
                "PUMA Unisex-Adult Popcat Slide Sandal",
                "https://m.media-amazon.com/images/I/61sMVp4N3RL.__AC_SY395_SX395_QL70_ML2_.jpg"
            },
            new object[]
            {
                "B086CX5XFV",
                null,
                3.4f,
                19,
                "ABEO Sammie - Low Heel Sandals",
                "https://m.media-amazon.com/images/I/61IxVJtEE+L._AC_SX395_SY395_.jpg"
            },
            new object[]
            {
                "B08S71Y7M7",
                "$299.00$299.00",
                4.2f,
                145,
                "Windows 10 Laptop Computers, CHUWI GemiBook 13 inch Thin &amp; Light Laptop 8G RAM 256GB SSD with Celeron J4125 Processor, 2160x1440 2K IPS Display Notebook, PD Fast Charge, Backlit Keyboard, Full Metal",
                "https://m.media-amazon.com/images/I/61NMZiXpjeL.__AC_SX300_SY300_QL70_ML2_.jpg"
            }
        };
}