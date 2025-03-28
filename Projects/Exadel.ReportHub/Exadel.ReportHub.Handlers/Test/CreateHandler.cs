using ErrorOr;
using MediatR;

namespace Exadel.ReportHub.Handlers.Test;

public record CreateRequest(string Name, bool GetError) : IRequest<ErrorOr<Created>>;

public class CreateHandler : IRequestHandler<CreateRequest, ErrorOr<Created>>
{
    public Task<ErrorOr<Created>> Handle(CreateRequest request, CancellationToken cancellationToken)
    {
        if (request.GetError)
        {
            return Task.FromResult<ErrorOr<Created>>(Error.Validation(description: "Test not valid."));
        }

        return Task.FromResult<ErrorOr<Created>>(Result.Created);
    }
}
