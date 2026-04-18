using AiOperationsHub.Api.Authorization;
using AiOperationsHub.Api.Contracts.Requests;
using AiOperationsHub.Api.Contracts.Responses;
using AiOperationsHub.Application.TargetResolution.Queries.ResolveTargetResource;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiOperationsHub.Api.Controllers
{
    /// <summary>
    /// Exposes endpoints for resolving user-provided target references into concrete resources.
    /// </summary>
    [ApiController]
    [Route("api/action-target-resolution")]
    public sealed class ActionTargetResolutionController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionTargetResolutionController"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender.</param>
        public ActionTargetResolutionController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Resolves a user-provided target reference into zero, one, or many concrete resources.
        /// </summary>
        /// <param name="request">The resolution request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolution result.</returns>
        [HttpPost("resolve")]
        [Authorize(Policy = AuthorizationPolicies.CanCreateProposals)]
        [ProducesResponseType(typeof(TargetResourceResolutionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TargetResourceResolutionResponse>> Resolve(
            [FromBody] ResolveTargetResourceRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new ResolveTargetResourceQuery
                {
                    TargetSystem = request.TargetSystem,
                    ScopeKey = request.ScopeKey,
                    Reference = request.Reference
                },
                cancellationToken);

            return Ok(TargetResourceResolutionResponse.FromModel(result));
        }
    }
}