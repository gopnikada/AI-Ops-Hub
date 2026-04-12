namespace AiOperationsHub.Api.Controllers
{
    using System.Security.Claims;
    using AiOperationsHub.Api.Contracts.SystemPrompts;
    using AiOperationsHub.Application.Prompts.Commands.UpsertSystemPrompt;
    using AiOperationsHub.Application.Prompts.Queries.GetSystemPromptByKey;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Exposes CRUD-style endpoints for persisted system prompts.
    /// </summary>
    [ApiController]
    [Route("api/system-prompts")]
    [Authorize(Roles = "Admin")]
    public sealed class SystemPromptsController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPromptsController"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender.</param>
        public SystemPromptsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Gets a system prompt by key.
        /// </summary>
        /// <param name="key">The prompt key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The stored prompt value.</returns>
        [HttpGet("{key}")]
        public async Task<IActionResult> GetAsync(
            string key,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetSystemPromptByKeyQuery
                {
                    Key = key
                },
                cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates or updates a system prompt by key.
        /// </summary>
        /// <param name="key">The prompt key.</param>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The saved prompt value.</returns>
        [HttpPut("{key}")]
        public async Task<IActionResult> PutAsync(
            string key,
            [FromBody] UpsertSystemPromptRequest request,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _sender.Send(
                new UpsertSystemPromptCommand
                {
                    Key = key,
                    Value = request.Value,
                    UpdatedByUserId = userId
                },
                cancellationToken);

            return Ok(result);
        }
    }
}