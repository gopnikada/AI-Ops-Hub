using AiOperationsHub.Api.Authorization;
using AiOperationsHub.Api.Contracts.Requests;
using AiOperationsHub.Api.Contracts.Responses;
using AiOperationsHub.Api.Extensions;
using AiOperationsHub.Application.Actions.Commands.ConfirmActionProposal;
using AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal;
using AiOperationsHub.Application.Actions.Commands.CreateJiraIssueProposal;
using AiOperationsHub.Application.Actions.Queries.GetActionProposalById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiOperationsHub.Api.Controllers
{
    /// <summary>
    /// Exposes HTTP endpoints for creating, retrieving, and confirming action proposals.
    /// </summary>
    [ApiController]
    [Route("api/action-proposals")]
    public sealed class ActionProposalsController : ControllerBase
    {
        private readonly ISender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionProposalsController"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender.</param>
        public ActionProposalsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Creates a Jira issue proposal.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The created proposal.</returns>
        [HttpPost("jira-issue")]
        [Authorize(Policy = AuthorizationPolicies.CanCreateProposals)]
        [ProducesResponseType(typeof(ActionProposalResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ActionProposalResponse>> CreateJiraIssueProposal(
            [FromBody] CreateJiraIssueProposalRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CreateJiraIssueProposalCommand
            {
                RequestedByUserId = User.GetRequiredUserId(),
                CorrelationId = Guid.NewGuid(),
                ConversationId = request.ConversationId,
                ProjectKey = request.ProjectKey,
                EpicKey = request.EpicKey,
                Summary = request.Summary,
                Description = request.Description,
                Assignee = request.Assignee
            };

            var proposal = await _sender.Send(command, cancellationToken);
            var response = ActionProposalResponse.FromDto(proposal);

            return CreatedAtAction(
                nameof(GetById),
                new { proposalId = response.Id },
                response);
        }

        /// <summary>
        /// Creates a Jira issue edit proposal, or returns concrete match candidates when target selection is required first.
        /// </summary>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The prepared proposal or target-resolution result.</returns>
        [HttpPost("jira-issue-edit")]
        [Authorize(Policy = AuthorizationPolicies.CanCreateProposals)]
        [ProducesResponseType(typeof(ProposalPreparationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProposalPreparationResponse>> CreateJiraIssueEditProposal(
            [FromBody] CreateJiraIssueEditProposalRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new CreateJiraIssueEditProposalCommand
                {
                    RequestedByUserId = User.GetRequiredUserId(),
                    CorrelationId = Guid.NewGuid(),
                    ConversationId = request.ConversationId,
                    ProjectKey = request.ProjectKey,
                    IssueReference = request.IssueReference,
                    ResolvedIssueKey = request.ResolvedIssueKey,
                    Summary = request.Summary,
                    Description = request.Description,
                    Assignee = request.Assignee,
                    Status = request.Status
                },
                cancellationToken);

            return Ok(ProposalPreparationResponse.FromDto(result));
        }

        /// <summary>
        /// Gets a proposal by identifier.
        /// </summary>
        /// <param name="proposalId">The proposal identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The proposal when found.</returns>
        [HttpGet("{proposalId:guid}")]
        [Authorize(Policy = AuthorizationPolicies.CanReadProposals)]
        [ProducesResponseType(typeof(ActionProposalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ActionProposalResponse>> GetById(
            [FromRoute] Guid proposalId,
            CancellationToken cancellationToken)
        {
            var proposal = await _sender.Send(
                new GetActionProposalByIdQuery
                {
                    ProposalId = proposalId
                },
                cancellationToken);

            if (proposal is null)
            {
                return NotFound();
            }

            return Ok(ActionProposalResponse.FromDto(proposal));
        }

        /// <summary>
        /// Confirms an existing proposal.
        /// </summary>
        /// <param name="proposalId">The proposal identifier.</param>
        /// <param name="request">The request payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated proposal.</returns>
        [HttpPost("{proposalId:guid}/confirm")]
        [Authorize(Policy = AuthorizationPolicies.CanConfirmProposals)]
        [ProducesResponseType(typeof(ActionProposalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ActionProposalResponse>> Confirm(
            [FromRoute] Guid proposalId,
            [FromBody] ConfirmActionProposalRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ConfirmActionProposalCommand
            {
                ProposalId = proposalId,
                ConfirmedByUserId = User.GetRequiredUserId(),
                CorrelationId = Guid.NewGuid(),
                ConversationId = request.ConversationId
            };

            var proposal = await _sender.Send(command, cancellationToken);
            return Ok(ActionProposalResponse.FromDto(proposal));
        }

        [HttpGet("debug/me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            }));
        }
    }
}