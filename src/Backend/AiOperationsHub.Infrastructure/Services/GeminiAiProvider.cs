using System.Net;
using System.Text;
using System.Text.Json;
using AiOperationsHub.Application.Abstractions.Providers;
using AiOperationsHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Provides a Gemini-backed implementation of <see cref="IAiProvider"/>.
    /// </summary>
    public sealed class GeminiAiProvider : IAiProvider
    {
        private static long _cumulativeTokenCount;

        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly ILogger<GeminiAiProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeminiAiProvider"/> class.
        /// </summary>
        /// <param name="httpClient">The configured HTTP client.</param>
        /// <param name="options">The Gemini options.</param>
        /// <param name="logger">The logger.</param>
        public GeminiAiProvider(
            HttpClient httpClient,
            IOptions<GeminiOptions> options,
            ILogger<GeminiAiProvider> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Generates provider output for the supplied request.
        /// </summary>
        /// <param name="request">The normalized AI provider request.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the normalized provider response.</returns>
        public async Task<AiProviderResponse> GenerateAsync(
            AiProviderRequest request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var requestUri =
                $"v1beta/models/{Uri.EscapeDataString(_options.Model)}:generateContent?key={Uri.EscapeDataString(_options.ApiKey)}";

            var payload = new
            {
                systemInstruction = string.IsNullOrWhiteSpace(request.SystemPrompt)
                    ? null
                    : new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = request.SystemPrompt
                            }
                        }
                    },
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new
                            {
                                text = request.UserPrompt
                            }
                        }
                    }
                }
            };

            var maxAttempts = 4;
            var delay = TimeSpan.FromSeconds(2);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(payload, CreateJsonOptions()),
                        Encoding.UTF8,
                        "application/json")
                };

                using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var outputText = ExtractOutputText(responseBody);
                    var usage = ExtractUsageMetadata(responseBody);

                    var cumulative = Interlocked.Add(ref _cumulativeTokenCount, usage.TotalTokenCount);

                    var requestPercent = CalculatePercent(usage.TotalTokenCount, _options.BudgetTokens);
                    var cumulativePercent = CalculatePercent(cumulative, _options.BudgetTokens);

                    _logger.LogInformation(
                        "Gemini request completed. CorrelationId: {CorrelationId}, Model: {Model}, PromptTokens: {PromptTokens}, OutputTokens: {OutputTokens}, TotalTokens: {TotalTokens}, CumulativeTokens: {CumulativeTokens}",
                        request.CorrelationId,
                        _options.Model,
                        usage.PromptTokenCount,
                        usage.OutputTokenCount,
                        usage.TotalTokenCount,
                        cumulative);

                    return new AiProviderResponse
                    {
                        OutputText = outputText,
                        RawResponseJson = responseBody,
                        PromptTokenCount = usage.PromptTokenCount,
                        OutputTokenCount = usage.OutputTokenCount,
                        TotalTokenCount = usage.TotalTokenCount,
                        CumulativeTokenCount = cumulative,
                        RequestPercentOfBudget = requestPercent,
                        CumulativePercentOfBudget = cumulativePercent
                    };
                }

                var isRetryable =
                    response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    (int)response.StatusCode == 429;

                if (!isRetryable || attempt == maxAttempts)
                {
                    _logger.LogError(
                        "Gemini request failed. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Attempt: {Attempt}, Response: {Response}",
                        (int)response.StatusCode,
                        request.CorrelationId,
                        attempt,
                        responseBody);

                    throw CreateProviderException(response.StatusCode, responseBody);
                }

                _logger.LogWarning(
                    "Gemini transient failure. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Attempt: {Attempt}. Retrying after {DelaySeconds}s.",
                    (int)response.StatusCode,
                    request.CorrelationId,
                    attempt,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2);
            }

            throw new InvalidOperationException("Gemini request failed after all retry attempts.");
        }

        private static string ExtractOutputText(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("candidates", out var candidatesElement) ||
                candidatesElement.ValueKind != JsonValueKind.Array)
            {
                throw new InvalidOperationException("Gemini response did not contain candidates.");
            }

            foreach (var candidate in candidatesElement.EnumerateArray())
            {
                if (!candidate.TryGetProperty("content", out var contentElement) ||
                    contentElement.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (!contentElement.TryGetProperty("parts", out var partsElement) ||
                    partsElement.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                var builder = new StringBuilder();

                foreach (var part in partsElement.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var textElement) &&
                        textElement.ValueKind == JsonValueKind.String)
                    {
                        var text = textElement.GetString();

                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            builder.Append(text);
                        }
                    }
                }

                var combined = builder.ToString();

                if (!string.IsNullOrWhiteSpace(combined))
                {
                    return combined;
                }
            }

            throw new InvalidOperationException("Gemini response did not contain any text output.");
        }

        private static GeminiUsageMetadata ExtractUsageMetadata(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("usageMetadata", out var usageElement) ||
                usageElement.ValueKind != JsonValueKind.Object)
            {
                return new GeminiUsageMetadata();
            }

            return new GeminiUsageMetadata
            {
                PromptTokenCount = GetInt32(usageElement, "promptTokenCount"),
                OutputTokenCount = GetInt32(usageElement, "candidatesTokenCount"),
                TotalTokenCount = GetInt32(usageElement, "totalTokenCount")
            };
        }

        private static int GetInt32(JsonElement parent, string propertyName)
        {
            if (!parent.TryGetProperty(propertyName, out var element) ||
                element.ValueKind != JsonValueKind.Number)
            {
                return 0;
            }

            return element.TryGetInt32(out var value) ? value : 0;
        }

        private static decimal CalculatePercent(long used, int budget)
        {
            if (budget <= 0)
            {
                return 0m;
            }

            return Math.Round((decimal)used / budget * 100m, 2, MidpointRounding.AwayFromZero);
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            return new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        private static Exception CreateProviderException(HttpStatusCode statusCode, string responseBody)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => new InvalidOperationException($"Gemini request was invalid. Response: {responseBody}"),
                HttpStatusCode.Unauthorized => new InvalidOperationException("Gemini API key is invalid or unauthorized."),
                HttpStatusCode.Forbidden => new InvalidOperationException("Gemini request was forbidden for the current project or key."),
                HttpStatusCode.TooManyRequests => new InvalidOperationException("Gemini rate limit was exceeded."),
                HttpStatusCode.ServiceUnavailable => new InvalidOperationException($"Gemini request failed with status code 503. Response: {responseBody}"),
                _ => new InvalidOperationException(
                    $"Gemini request failed with status code {(int)statusCode}. Response: {responseBody}")
            };
        }

        private sealed class GeminiUsageMetadata
        {
            public int PromptTokenCount { get; set; }
            public int OutputTokenCount { get; set; }
            public int TotalTokenCount { get; set; }
        }
    }
}