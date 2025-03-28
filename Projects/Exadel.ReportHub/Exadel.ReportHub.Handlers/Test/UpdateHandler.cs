using ErrorOr;
using MediatR;

namespace Exadel.ReportHub.Handlers.Test;

public record UpdateRequest(Guid Id, string Name, bool GetError) : IRequest<ErrorOr<Updated>>;

public class UpdateHandler : IRequestHandler<UpdateRequest, ErrorOr<Updated>>
{
    public Task<ErrorOr<Updated>> Handle(UpdateRequest request, CancellationToken cancellationToken)
    {
        if (request.GetError)
        {
            return Task.FromResult<ErrorOr<Updated>>(Error.Validation(description: "Test not valid."));
        }

        return Task.FromResult<ErrorOr<Updated>>(Result.Updated);
    }
}
