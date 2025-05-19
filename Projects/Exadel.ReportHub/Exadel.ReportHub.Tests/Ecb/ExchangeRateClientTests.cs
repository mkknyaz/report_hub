using System.Net;
using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Tests.Abstracts;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Exadel.ReportHub.Tests.Ecb;

[TestFixture]
public class ExchangeRateClientTests : BaseTestFixture
{
    private const string Currency = "USD";
    private const string ValidXmlResponse = @"
            <GenericData xmlns:generic='http://www.ecb.com'>
                <generic:Series>
                    <generic:Obs>
                        <generic:ObsDimension value='2023-01-01'/>
                        <generic:ObsValue value='1.2345'/>
                    </generic:Obs>
                    <generic:Obs>
                        <generic:ObsDimension value='2023-01-02'/>
                        <generic:ObsValue value='1.2356'/>
                    </generic:Obs>
                </generic:Series>
            </GenericData>";

    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<ILogger<ExchangeRateClient>> _loggerMock;
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private ExchangeRateClient _client;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _loggerMock = new Mock<ILogger<ExchangeRateClient>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://test.com/")
        };

        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        _client = new ExchangeRateClient(_httpClientFactoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GetByCurrencyInPeriodAsync_ValidResponse_ReturnsRates()
    {
        // Arrange
        var startDate = Fixture.Create<DateTime>();
        var endDate = startDate.AddDays(7);

        SetupHttpResponse(HttpStatusCode.OK, ValidXmlResponse);

        // Act
        var result = await _client.GetByCurrencyInPeriodAsync(Currency, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Rate, Is.EqualTo(1.2345m));
        Assert.That(result[0].RateDate.Date, Is.EqualTo(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc).Date));
        Assert.That(result[1].RateDate.Date, Is.EqualTo(new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc).Date));
        Assert.That(result[1].Rate, Is.EqualTo(1.2356m));
        Assert.That(result, Has.All.Matches<ExchangeRate>(x => x.Currency == Currency));
    }

    [Test]
    public async Task GetByCurrencyInPeriodAsync_InvalidXml_ReturnsEmptyList()
    {
        // Arrange
        var startDate = Fixture.Create<DateTime>();
        var endDate = startDate.AddDays(7);
        var invalidXml = Fixture.Create<string>();

        SetupHttpResponse(HttpStatusCode.OK, invalidXml);

        // Act
        var result = await _client.GetByCurrencyInPeriodAsync(Currency, startDate, endDate, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
        VerifyErrorLogged(Constants.Error.EmptyXml);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }

    private void VerifyErrorLogged(string errorMessage)
    {
        _loggerMock.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(errorMessage, StringComparison.Ordinal)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
