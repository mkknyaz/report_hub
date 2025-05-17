using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Common.Exceptions;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Client.Create;
using Exadel.ReportHub.Host.Mediatr;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Host.MediatR;

[TestFixture]
public class RequestValidationBehaviorTests : BaseTestFixture
{
    private Mock<IValidator<CreateClientRequest>> _validatorMock;
    private RequestValidationBehavior<CreateClientRequest, ErrorOr<ClientDTO>> _behavior;
    private List<IValidator<CreateClientRequest>> _validators;
    private Mock<RequestHandlerDelegate<ErrorOr<ClientDTO>>> _nextMock;

    [SetUp]
    public void Setup()
    {
        _validatorMock = new Mock<IValidator<CreateClientRequest>>();
        _validators = new List<IValidator<CreateClientRequest>> { _validatorMock.Object };
        _behavior = new RequestValidationBehavior<CreateClientRequest, ErrorOr<ClientDTO>>(_validators);
        _nextMock = new Mock<RequestHandlerDelegate<ErrorOr<ClientDTO>>>();
    }

    [Test]
    public async Task Handle_ValidRequest_CallsNext()
    {
        // Arrange
        var validDto = Fixture.Create<CreateClientDTO>();

        var request = new CreateClientRequest(validDto);

        _validatorMock.Setup(x => x.ValidateAsync(request, CancellationToken.None))
            .ReturnsAsync(new ValidationResult());

        var expectedResponse = Fixture.Build<ClientDTO>()
            .With(x => x.Name, validDto.Name)
            .Create();

        _nextMock.Setup(x => x()).ReturnsAsync(ErrorOrFactory.From(expectedResponse));

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Name, Is.EqualTo(validDto.Name));
        _validatorMock.Verify(x => x.ValidateAsync(request, CancellationToken.None), Times.Once);
        _nextMock.Verify(x => x(), Times.Once);
    }

    [Test]
    public void Handle_InvalidBankAccountNumber_ThrowsWithValidationErrors()
    {
        // Arrange
        var invalidDto = Fixture.Build<CreateClientDTO>()
            .With(x => x.BankAccountNumber, "123423432432432")
            .Create();

        var failures = new List<ValidationFailure>
        {
            new(nameof(CreateClientDTO.BankAccountNumber), Constants.Validation.BankAccountNumber.InvalidFormat)
        };

        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateClientRequest>(), CancellationToken.None))
            .ReturnsAsync(new ValidationResult(failures));

        var request = new CreateClientRequest(invalidDto);

        // Act & Assert
        var result = Assert.ThrowsAsync<HttpStatusCodeException>(async () =>
        {
            await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);
        });

        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        Assert.That(result.Errors, Has.Count.EqualTo(1));
        Assert.That(result.Errors[0], Is.EqualTo(Constants.Validation.BankAccountNumber.InvalidFormat));
        _nextMock.Verify(x => x(), Times.Never);
    }
}
