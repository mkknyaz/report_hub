using Exadel.ReportHub.Handlers.Test;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Exadel.ReportHub.Host.Services;

public class TestService(ISender sender) : BaseService
{
    [HttpGet]
    public IActionResult GetSampleAnswer()
    {
        return Ok();
    }

    [HttpGet("{getError}")]
    public async Task<IActionResult> GetTest([FromRoute] GetRequest request)
    {
        var result = await sender.Send(request);

        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddTest([FromBody] CreateRequest request)
    {
        var result = await sender.Send(request);

        return FromResult(result);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateTest([FromRoute] Guid id, [FromBody] CreateRequest request)
    {
        var updateRequest = new UpdateRequest(id, request.Name, request.GetError);
        var result = await sender.Send(updateRequest);

        return FromResult(result);
    }

    [HttpDelete("{id:guid}/{getError}")]
    public async Task<IActionResult> DeleteTest([FromRoute] DeleteRequest request)
    {
        var result = await sender.Send(request);

        return FromResult(result);
    }
}
