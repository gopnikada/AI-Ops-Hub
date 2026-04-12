namespace AiOperationsHub.Api.Controllers
{
    using System.Security.Claims;
    using System.Text.Json;
    using AiOperationsHub.Api.Contracts.Chat;
    using AiOperationsHub.Application.Chat;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Exposes generic chat endpoints backed by the internal tool orchestration workflow.
    /// </summary>
    [ApiController]
    [Route("api/chat")]
    [Authorize(Roles = "Admin,Operator")]
    public sealed class ChatController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions =
            new(JsonSerializerDefaults.Web);

        private readonly IChatOrchestrator _chatOrchestrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatController"/> class.
        /// </summary>
        /// <param name="chatOrchestrator">The chat orchestrator.</param>
        public ChatController(IChatOrchestrator chatOrchestrator)
        {
            _chatOrchestrator = chatOrchestrator;
        }

        /// <summary>
        /// Streams structured chat events as newline-delimited JSON.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("stream")]
        [Produces("application/x-ndjson")]
        public async Task StreamAsync(
            [FromBody] ChatStreamRequestContract request,
            CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            Response.ContentType = "application/x-ndjson";
            Response.StatusCode = StatusCodes.Status200OK;

            await Response.StartAsync(cancellationToken);

            var correlationId = Guid.NewGuid();

            await foreach (var evt in _chatOrchestrator.StreamAsync(
                new ChatStreamRequest
                {
                    ConversationId = request.ConversationId,
                    Message = request.Message
                },
                userId,
                correlationId,
                cancellationToken))
            {
                var line = JsonSerializer.Serialize(evt, JsonOptions) + "\n";

                await Response.WriteAsync(line, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
    }
}