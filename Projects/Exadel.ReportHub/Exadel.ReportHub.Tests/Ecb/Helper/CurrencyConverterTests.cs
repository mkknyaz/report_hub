using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Ecb.Helpers;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Ecb.Helper;

[TestFixture]
public class CurrencyConverterTests : BaseTestFixture
{
    private const decimal Amount = 100m;

    private Mock<IExchangeRateProvider> _exchangeRateProviderMock;
    private CurrencyConverter _converter;

    [SetUp]
    public void SetUp()
    {
        _exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        _converter = new CurrencyConverter(_exchangeRateProviderMock.Object);
    }

    [Test]
    public async Task ConvertAsync_SameCurrency_ReturnsSameAmount()
    {
        // Arrange
        var amount = Fixture.Create<decimal>();
        var currency = "USD";
        var date = Fixture.Create<DateTime>();

        // Act
        var result = await _converter.ConvertAsync(amount, currency, currency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(amount));
    }

    [Test]
    public async Task ConvertAsync_FromDefaultToTargetCurrency_ConvertsCorrectly()
    {
        // Arrange
        var fromCurrency = "EUR";
        var toCurrency = "USD";
        var rate = 1.2m;
        var date = Fixture.Create<DateTime>();

        _exchangeRateProviderMock
            .Setup(p => p.GetByCurrencyForWeekAsync(toCurrency, date, CancellationToken.None))
            .ReturnsAsync(new ExchangeRate { Rate = rate });

        // Act
        var result = await _converter.ConvertAsync(Amount, fromCurrency, toCurrency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Amount * rate));
    }

    [Test]
    public async Task ConvertAsync_FromCurrencyToDefaultCurrency_ConvertsCorrectly()
    {
        // Arrange
        var fromCurrency = "USD";
        var toCurrency = "EUR";
        var rate = 0.80m;
        var date = Fixture.Create<DateTime>();

        _exchangeRateProviderMock
            .Setup(p => p.GetByCurrencyForWeekAsync(fromCurrency, date, CancellationToken.None))
            .ReturnsAsync(new ExchangeRate { Rate = rate });

        // Act
        var result = await _converter.ConvertAsync(Amount, fromCurrency, toCurrency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Amount / rate));
    }

    [Test]
    public async Task ConvertAsync_NonDefaultCurrencies_ConvertsCorrectly()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var fromRate = 1.5m;
        var toRate = 1.2m;
        var date = Fixture.Create<DateTime>();

        _exchangeRateProviderMock
            .Setup(p => p.GetByCurrencyForWeekAsync(fromCurrency, date, CancellationToken.None))
            .ReturnsAsync(new ExchangeRate { Rate = fromRate });

        _exchangeRateProviderMock
            .Setup(p => p.GetByCurrencyForWeekAsync(toCurrency, date, CancellationToken.None))
            .ReturnsAsync(new ExchangeRate { Rate = toRate });

        // Act
        var result = await _converter.ConvertAsync(Amount, fromCurrency, toCurrency, date, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Amount * toRate / fromRate));
    }
}
