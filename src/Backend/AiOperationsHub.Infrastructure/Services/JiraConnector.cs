using AiOperationsHub.Application.Abstractions.Jira;
using AiOperationsHub.Application.Common.Models;
using AiOperationsHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

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
        /// Searches Jira issues using a user-provided reference and returns zero, one, or many matches.
        /// </summary>
        /// <param name="request">The issue lookup request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The matched Jira issues.</returns>
        public async Task<IReadOnlyCollection<ResolvedJiraIssueResponse>> SearchIssuesAsync(
            ResolveJiraIssueRequest request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var reference = request.IssueReference.Trim();

            if (LooksLikeIssueKey(reference))
            {
                var exactIssue = await GetIssueAsync(reference, cancellationToken);

                return new[]
                {
                    new ResolvedJiraIssueResponse
                    {
                        IssueKey = exactIssue.IssueKey,
                        Summary = exactIssue.Summary,
                        Description = exactIssue.Description,
                        IssueUrl = $"{_options.BaseUrl.TrimEnd('/')}/browse/{exactIssue.IssueKey}"
                    }
                };
            }

            string jql;

            if (LooksLikeProjectKeyReference(reference) && string.IsNullOrWhiteSpace(request.ProjectKey))
            {
                jql = $"project = \"{EscapeJql(reference)}\" order by updated desc";
            }
            else
            {
                var scopeClause = string.IsNullOrWhiteSpace(request.ProjectKey)
                    ? string.Empty
                    : $"project = \"{EscapeJql(request.ProjectKey)}\" AND ";

                jql = $"{scopeClause}text ~ \"\\\"{EscapeJql(reference)}\\\"\" order by updated desc";
            }

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "rest/api/3/search/jql");
            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    jql,
                    maxResults = 10,
                    fields = new[] { "summary", "description" }
                }),
                Encoding.UTF8,
                "application/json");

            ApplyAuthentication(httpRequest);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Jira issue search failed with status code {(int)response.StatusCode}. Response: {raw}");
            }

            using var document = JsonDocument.Parse(raw);
            var issues = document.RootElement.GetProperty("issues");

            var matches = new List<ResolvedJiraIssueResponse>();

            foreach (var issue in issues.EnumerateArray())
            {
                var issueKey = issue.GetProperty("key").GetString();

                if (string.IsNullOrWhiteSpace(issueKey))
                {
                    continue;
                }

                var fields = issue.GetProperty("fields");

                matches.Add(new ResolvedJiraIssueResponse
                {
                    IssueKey = issueKey,
                    Summary = fields.TryGetProperty("summary", out var summaryElement)
                        ? summaryElement.GetString() ?? string.Empty
                        : string.Empty,
                    Description = TryReadAdfAsPlainText(fields, "description"),
                    IssueUrl = $"{_options.BaseUrl.TrimEnd('/')}/browse/{issueKey}"
                });
            }

            return matches;
        }

        /// <inheritdoc />
        public async Task<ResolvedJiraIssueResponse> ResolveIssueAsync(
            ResolveJiraIssueRequest request,
            CancellationToken cancellationToken)
        {
            var matches = await SearchIssuesAsync(request, cancellationToken);

            if (matches.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No Jira issue could be resolved from reference '{request.IssueReference}'.");
            }

            if (matches.Count > 1)
            {
                var keys = matches
                    .Select(x => x.IssueKey)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();

                throw new InvalidOperationException(
                    $"More than one Jira issue matched reference '{request.IssueReference}'. Matches: {string.Join(", ", keys)}");
            }

            return matches[0];
        }

        /// <inheritdoc />
        public async Task<JiraIssueDetailsResponse> GetIssueAsync(
            string issueKey,
            CancellationToken cancellationToken)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"rest/api/3/issue/{Uri.EscapeDataString(issueKey)}?fields=summary,description,assignee,status");

            ApplyAuthentication(httpRequest);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Jira issue read failed for '{issueKey}' with status code {(int)response.StatusCode}. Response: {raw}");
            }

            using var document = JsonDocument.Parse(raw);
            var fields = document.RootElement.GetProperty("fields");

            return new JiraIssueDetailsResponse
            {
                IssueKey = document.RootElement.GetProperty("key").GetString() ?? string.Empty,
                Summary = fields.GetProperty("summary").GetString() ?? string.Empty,
                Description = TryReadAdfAsPlainText(fields, "description"),
                Assignee = TryReadNestedString(fields, "assignee", "displayName"),
                Status = TryReadNestedString(fields, "status", "name") ?? string.Empty
            };
        }

        /// <inheritdoc />
        public async Task<UpdateJiraIssueResult> UpdateIssueAsync(
            UpdateJiraIssueRequest request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!string.IsNullOrWhiteSpace(request.Summary) ||
                !string.IsNullOrWhiteSpace(request.Description) ||
                !string.IsNullOrWhiteSpace(request.Assignee))
            {
                var fields = new Dictionary<string, object?>();

                if (!string.IsNullOrWhiteSpace(request.Summary))
                {
                    fields["summary"] = request.Summary;
                }

                if (!string.IsNullOrWhiteSpace(request.Description))
                {
                    fields["description"] = CreateAdfTextDocument(request.Description);
                }

                if (request.Assignee is not null)
                {
                    fields["assignee"] = string.IsNullOrWhiteSpace(request.Assignee)
                        ? null
                        : new { accountId = request.Assignee };
                }

                using var editRequest = new HttpRequestMessage(
                    HttpMethod.Put,
                    $"rest/api/3/issue/{Uri.EscapeDataString(request.IssueKey)}");

                editRequest.Content = new StringContent(
                    JsonSerializer.Serialize(new { fields }),
                    Encoding.UTF8,
                    "application/json");

                ApplyAuthentication(editRequest);

                using var editResponse = await _httpClient.SendAsync(editRequest, cancellationToken);
                var editRaw = await editResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!editResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Jira issue edit failed for '{request.IssueKey}' with status code {(int)editResponse.StatusCode}. Response: {editRaw}");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                using var transitionsLookupRequest = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"rest/api/3/issue/{Uri.EscapeDataString(request.IssueKey)}/transitions");

                ApplyAuthentication(transitionsLookupRequest);

                using var transitionsResponse = await _httpClient.SendAsync(transitionsLookupRequest, cancellationToken);
                var transitionsRaw = await transitionsResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!transitionsResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Jira transitions lookup failed for '{request.IssueKey}' with status code {(int)transitionsResponse.StatusCode}. Response: {transitionsRaw}");
                }

                using var transitionsDocument = JsonDocument.Parse(transitionsRaw);
                var transition = transitionsDocument.RootElement
                    .GetProperty("transitions")
                    .EnumerateArray()
                    .FirstOrDefault(x =>
                        string.Equals(
                            x.GetProperty("to").GetProperty("name").GetString(),
                            request.Status,
                            StringComparison.OrdinalIgnoreCase));

                if (transition.ValueKind == JsonValueKind.Undefined)
                {
                    throw new InvalidOperationException(
                        $"No Jira transition to status '{request.Status}' is currently available for issue '{request.IssueKey}'.");
                }

                var transitionId = transition.GetProperty("id").GetString();

                using var transitionRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"rest/api/3/issue/{Uri.EscapeDataString(request.IssueKey)}/transitions");

                transitionRequest.Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        transition = new
                        {
                            id = transitionId
                        }
                    }),
                    Encoding.UTF8,
                    "application/json");

                ApplyAuthentication(transitionRequest);

                using var transitionResponse = await _httpClient.SendAsync(transitionRequest, cancellationToken);
                var transitionRaw = await transitionResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!transitionResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Jira issue transition failed for '{request.IssueKey}' with status code {(int)transitionResponse.StatusCode}. Response: {transitionRaw}");
                }
            }

            return new UpdateJiraIssueResult
            {
                IssueKey = request.IssueKey,
                IssueUrl = $"{_options.BaseUrl.TrimEnd('/')}/browse/{request.IssueKey}",
                RawResponseJson = JsonSerializer.Serialize(new
                {
                    request.IssueKey,
                    request.Summary,
                    request.Description,
                    request.Assignee,
                    request.Status
                })
            };
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

        private static bool LooksLikeIssueKey(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                value.Trim(),
                "^[A-Z][A-Z0-9_]*-\\d+$");
        }

        private static bool LooksLikeProjectKeyReference(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(
                value.Trim(),
                "^[A-Z][A-Z0-9_]*$");
        }

        private static string EscapeJql(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static string? TryReadNestedString(JsonElement parent, string propertyName, string nestedPropertyName)
        {
            if (!parent.TryGetProperty(propertyName, out var property) ||
                property.ValueKind == JsonValueKind.Null ||
                property.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (!property.TryGetProperty(nestedPropertyName, out var nested))
            {
                return null;
            }

            return nested.GetString();
        }

        private static string? TryReadAdfAsPlainText(JsonElement parent, string propertyName)
        {
            if (!parent.TryGetProperty(propertyName, out var property) ||
                property.ValueKind == JsonValueKind.Null ||
                property.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (!property.TryGetProperty("content", out var content) ||
                content.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var parts = new List<string>();

            foreach (var block in content.EnumerateArray())
            {
                if (!block.TryGetProperty("content", out var inlineContent) ||
                    inlineContent.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var inline in inlineContent.EnumerateArray())
                {
                    if (inline.TryGetProperty("text", out var text) &&
                        text.ValueKind == JsonValueKind.String)
                    {
                        parts.Add(text.GetString() ?? string.Empty);
                    }
                }
            }

            return string.Join(" ", parts).Trim();
        }

        private static object CreateAdfTextDocument(string text)
        {
            return new
            {
                type = "doc",
                version = 1,
                content = new object[]
                {
                    new
                    {
                        type = "paragraph",
                        content = new object[]
                        {
                            new
                            {
                                type = "text",
                                text
                            }
                        }
                    }
                }
            };
        }
    }
}