using ErrorOr;
using MediatR;

namespace Exadel.ReportHub.Handlers.Test;

public record DeleteRequest(Guid Id, bool GetError) : IRequest<ErrorOr<Deleted>>;

public class DeleteHandler : IRequestHandler<DeleteRequest, ErrorOr<Deleted>>
{
    public Task<ErrorOr<Deleted>> Handle(DeleteRequest request, CancellationToken cancellationToken)
    {
        if (request.GetError)
        {
            return Task.FromResult<ErrorOr<Deleted>>(Error.NotFound(description: "Test does not exist."));
        }

        return Task.FromResult<ErrorOr<Deleted>>(Result.Deleted);
    }
}
