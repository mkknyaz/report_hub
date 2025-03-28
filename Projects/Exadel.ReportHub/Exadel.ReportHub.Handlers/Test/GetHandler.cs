using ErrorOr;
using MediatR;

namespace Exadel.ReportHub.Handlers.Test;

public record GetRequest(bool GetError) : IRequest<ErrorOr<string>>;

public class GetHandler : IRequestHandler<GetRequest, ErrorOr<string>>
{
    public Task<ErrorOr<string>> Handle(GetRequest request, CancellationToken cancellationToken)
    {
        if (request.GetError)
        {
            return Task.FromResult<ErrorOr<string>>(Error.NotFound(description: "Test does not exist."));
        }

        return Task.FromResult<ErrorOr<string>>("Get test success.");
    }
}
