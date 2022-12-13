using Application.Facades;
using Application.Ports;
using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using Xunit;
using Application.Commands;
using FluentAssertions;
using Application.Models.Enums;

namespace UnitTests.Commands;

public class GetProductByAsinTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IAmazonScrapperFacade> _amazonScrapperFacadeMock;

    public GetProductByAsinTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _amazonScrapperFacadeMock = _fixture.Freeze<Mock<IAmazonScrapperFacade>>();
    }

    [Fact]
    public async Task GetProductByAsin_MoreThan10Items_Error()
    {
        var asins = "1,2,3,4,5,6,7,8,9,10,11";
        var sut = _fixture.Create<GetProductByAsin.Handler>();
        var response = await sut.Handle(new GetProductByAsin.Command(asins), default);
        response.Errors.Should().HaveCount(1);
        response.Errors.Single().ErrorMessage.Should().Be("Asin list limited to 10 items");
        response.Errors.Single().ErrorCode.Should().Be(ErrorCodes.BadRequest);
    }

    [Fact]
    public async Task GetProductByAsin_10Items_NoError()
    {
        var asins = "1,2,3,4,5,6,7,8,9,10";
        var sut = _fixture.Create<GetProductByAsin.Handler>();
        var response = await sut.Handle(new GetProductByAsin.Command(asins), default);
        response.Errors.Should().BeEmpty();
    }
}