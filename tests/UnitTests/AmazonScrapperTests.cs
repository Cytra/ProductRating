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

    [Theory]
    [MemberData(nameof(GetProductSearch))]
    public async Task GetProductsBySearchTerm(
        string searchTerm,
        int page,
        int pageSize,
        int totalPages,
        string asin,
        string description,
        int numReviews,
        string price,
        float rating,
        bool sponsored)
    {
        var rootFolder = "Data";
        var subFolder = "ProductSearch";
        var path = Path.Combine(rootFolder, subFolder, $"{searchTerm}.html");
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.Combine(assemblyPath, path);
        var rawHtml = File.ReadAllText(fullPath);

        _amazonHttpClientMock.Setup(x => 
                x.SearchProducts(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(rawHtml);

        var sut = _fixture.Create<AmazonScrapper>();

        var products = await sut.GetProductsBySearchTerm(searchTerm, 1);
        products.Items.Should().NotBeNull();
        var firstProduct = products.Items.First();
        firstProduct.Asin.Should().Be(asin);
        firstProduct.Description.Should()
            .Be(description);
        firstProduct.NumReviews.Should().Be(numReviews);
        firstProduct.Price.Should().Be(price);
        firstProduct.Rating.Should().Be(rating);
        firstProduct.Sponsored.Should().Be(sponsored);
        products.Should().NotBeNull();
        products.Paging.Page.Should().Be(page);
        products.Paging.PageSize.Should().Be(pageSize);
        products.Paging.TotalPages.Should().Be(totalPages);
    }

    public static IEnumerable<object[]> GetProductSearch =>
        new List<object[]>
        {
            new object[]
            {
                "DellLaptops",
                1,
                22,
                20,
                "B0BN7HZQ2M",
                "Dell 2023 Newest Inspiron 3000 15.6&quot; FHD Touchscreen Laptop, Intel i5-1135G7 Up to 4.2GHz, Beat i7-1060G7, 16GB DDR4 RAM, 512GB PCIe SSD, SD Card Reader, Webcam, HDMI, Windows 11 Home, Black",
                1,
                "$599.99",
                1f,
                true
            },
            new object[]
            {
                "WomenGifts",
                1,
                60,
                7,
                "B01DWH11L4",
                "VIKTOR JURGEN Neck Massage Pillow Shiatsu Deep Kneading Shoulder Back and Foot Massager with Heat-Relaxation Gifts for Women/Men/Dad/Mom",
                19424,
                "$49.98",
                4.4F,
                true
            },
        };

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
                "$299.00",
                4.2f,
                145,
                "Windows 10 Laptop Computers, CHUWI GemiBook 13 inch Thin &amp; Light Laptop 8G RAM 256GB SSD with Celeron J4125 Processor, 2160x1440 2K IPS Display Notebook, PD Fast Charge, Backlit Keyboard, Full Metal",
                "https://m.media-amazon.com/images/I/61NMZiXpjeL.__AC_SX300_SY300_QL70_ML2_.jpg"
            }
        };
}