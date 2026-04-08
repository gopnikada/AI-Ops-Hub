using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiOperationsHub.Application.Abstractions.Jira;
using AiOperationsHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Provides a real Jira connector implementation backed by Jira REST APIs.
    /// </summary>
    public sealed class JiraConnector : IJiraConnector
    {
        private readonly HttpClient _httpClient;
        private readonly JiraOptions _options;
        private readonly ILogger<JiraConnector> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraConnector"/> class.
        /// </summary>
        /// <param name="httpClient">The configured HTTP client.</param>
        /// <param name="options">The Jira integration options.</param>
        /// <param name="logger">The logger.</param>
        public JiraConnector(
            HttpClient httpClient,
            IOptions<JiraOptions> options,
            ILogger<JiraConnector> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Creates a Jira issue using the supplied normalized request.
        /// </summary>
        /// <param name="request">The normalized Jira issue creation request.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the Jira issue creation result.</returns>
        public async Task<CreateJiraIssueResult> CreateIssueAsync(
            CreateJiraIssueDraftRequest request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var payload = BuildCreateIssuePayload(request);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rest/api/3/issue");
            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            ApplyAuthentication(httpRequest);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Jira issue creation failed. StatusCode: {StatusCode}, ProjectKey: {ProjectKey}, Response: {Response}",
                    (int)response.StatusCode,
                    request.ProjectKey,
                    responseBody);

                throw new InvalidOperationException(
                    $"Jira issue creation failed with status code {(int)response.StatusCode}. Response: {responseBody}");
            }

            var result = ParseCreateIssueResult(responseBody);

            _logger.LogInformation(
                "Jira issue created successfully. IssueKey: {IssueKey}, ProjectKey: {ProjectKey}",
                result.IssueKey,
                request.ProjectKey);

            return result;
        }

        /// <summary>
        /// Builds the Jira REST API payload for issue creation.
        /// </summary>
        /// <param name="request">The normalized request.</param>
        /// <returns>The serialized request object payload.</returns>
        private static object BuildCreateIssuePayload(CreateJiraIssueDraftRequest request)
        {
            var fields = new Dictionary<string, object?>
            {
                ["project"] = new { key = request.ProjectKey },
                ["summary"] = request.Summary,
                ["description"] = new
                {
                    type = "doc",
                    version = 1,
                    content = new[]
                    {
                        new
                        {
                            type = "paragraph",
                            content = new[]
                            {
                                new
                                {
                                    type = "text",
                                    text = request.Description
                                }
                            }
                        }
                    }
                },
                ["issuetype"] = new { name = "Task" }
            };

            if (!string.IsNullOrWhiteSpace(request.Assignee))
            {
                fields["assignee"] = new { id = request.Assignee };
            }

            if (!string.IsNullOrWhiteSpace(request.EpicKey))
            {
                fields["parent"] = new { key = request.EpicKey };
            }

            return new
            {
                fields
            };
        }

        /// <summary>
        /// Parses the Jira create issue response into the application result contract.
        /// </summary>
        /// <param name="responseBody">The raw response body.</param>
        /// <returns>The parsed issue creation result.</returns>
        private CreateJiraIssueResult ParseCreateIssueResult(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            var issueKey = root.TryGetProperty("key", out var keyElement)
                ? keyElement.GetString()
                : null;

            if (string.IsNullOrWhiteSpace(issueKey))
            {
                throw new InvalidOperationException("Jira response did not contain an issue key.");
            }

            var issueUrl = $"{_options.BaseUrl.TrimEnd('/')}/browse/{issueKey}";

            return new CreateJiraIssueResult
            {
                IssueKey = issueKey,
                IssueUrl = issueUrl,
                RawResponseJson = responseBody
            };
        }

        /// <summary>
        /// Applies configured authentication headers to the outbound Jira request.
        /// </summary>
        /// <param name="request">The outbound HTTP request.</param>
        private void ApplyAuthentication(HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(_options.Email) || string.IsNullOrWhiteSpace(_options.ApiToken))
            {
                throw new InvalidOperationException("Jira credentials are not configured.");
            }

            var rawCredentials = $"{_options.Email}:{_options.ApiToken}";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(rawCredentials));

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}