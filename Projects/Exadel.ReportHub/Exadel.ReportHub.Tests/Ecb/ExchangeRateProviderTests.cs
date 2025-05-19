using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Ecb.Extensions;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Ecb;

[TestFixture]
public class ExchangeRateProviderTests : BaseTestFixture
{
    private const string Currency = "USD";

    private Mock<IExchangeRateRepository> _exchangeRateRepositoryMock;
    private Mock<IExchangeRateClient> _exchangeRateClientMock;
    private ExchangeRateProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _exchangeRateRepositoryMock = new Mock<IExchangeRateRepository>();
        _exchangeRateClientMock = new Mock<IExchangeRateClient>();
        _provider = new ExchangeRateProvider(_exchangeRateRepositoryMock.Object, _exchangeRateClientMock.Object);
    }

    [Test]
    public async Task GetByCurrencyForWeekAsync_ExchangeRateExists_ReturnsExchangeRateFromRepository()
    {
        // Arrange
        var date = DateTime.Today;
        var expectedRate = Fixture.Create<ExchangeRate>();

        _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyAsync(Currency, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedRate);

        // Act
        var result = await _provider.GetByCurrencyForWeekAsync(Currency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRate));

        _exchangeRateClientMock.Verify(c => c.GetByCurrencyInPeriodAsync(
            It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None), Times.Never);
    }

    [Test]
    public async Task GetByCurrencyForWeekAsync_RepoReturnsNull_FetchesAndUpsertsRatesFromClient()
    {
        // Arrange
        var date = DateTime.Today;
        var weekStartDate = date.GetWeekPeriodStart();
        var rates = Fixture.CreateMany<ExchangeRate>(3).ToList();

        _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyAsync(Currency, date, CancellationToken.None))
            .ReturnsAsync((ExchangeRate)null);

        _exchangeRateClientMock.Setup(c => c.GetByCurrencyInPeriodAsync(Currency, weekStartDate, date, CancellationToken.None))
            .ReturnsAsync(rates);

        // Act
        var result = await _provider.GetByCurrencyForWeekAsync(Currency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(rates.MaxBy(r => r.RateDate)));
        _exchangeRateRepositoryMock.Verify(r => r.UpsertManyAsync(rates, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task GetByCurrencyForWeekAsync_ClientReturnsEmptyForWeeksLimit_ReturnsNull()
    {
        // Arrange
        var date = DateTime.Today;

        _exchangeRateRepositoryMock.Setup(r => r.GetByCurrencyAsync(Currency, date, CancellationToken.None))
            .ReturnsAsync((ExchangeRate)null);

        _exchangeRateClientMock.Setup(c => c.GetByCurrencyInPeriodAsync(
            It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None))
            .ReturnsAsync(new List<ExchangeRate>());

        // Act
        var result = await _provider.GetByCurrencyForWeekAsync(Currency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
        _exchangeRateRepositoryMock.Verify(r => r.UpsertManyAsync(It.IsAny<IEnumerable<ExchangeRate>>(), CancellationToken.None), Times.Never);
    }
}
