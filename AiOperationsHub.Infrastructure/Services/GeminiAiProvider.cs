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

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload, CreateJsonOptions()),
                    Encoding.UTF8,
                    "application/json")
            };

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Gemini request failed. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Response: {Response}",
                    (int)response.StatusCode,
                    request.CorrelationId,
                    responseBody);

                throw CreateProviderException(response.StatusCode, responseBody);
            }

            var outputText = ExtractOutputText(responseBody);

            _logger.LogInformation(
                "Gemini request completed successfully. CorrelationId: {CorrelationId}, ProviderType: {ProviderType}, Model: {Model}",
                request.CorrelationId,
                request.ProviderType,
                _options.Model);

            return new AiProviderResponse
            {
                OutputText = outputText,
                RawResponseJson = responseBody
            };
        }

        /// <summary>
        /// Extracts the primary text output from a Gemini generateContent response payload.
        /// </summary>
        /// <param name="responseBody">The raw JSON response body.</param>
        /// <returns>The extracted text output.</returns>
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

        /// <summary>
        /// Creates provider-specific JSON serialization options.
        /// </summary>
        /// <returns>The JSON serializer options.</returns>
        private static JsonSerializerOptions CreateJsonOptions()
        {
            return new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// Creates an exception appropriate to the provider HTTP failure response.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="responseBody">The raw response body.</param>
        /// <returns>The mapped exception.</returns>
        private static Exception CreateProviderException(HttpStatusCode statusCode, string responseBody)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => new InvalidOperationException($"Gemini request was invalid. Response: {responseBody}"),
                HttpStatusCode.Unauthorized => new InvalidOperationException("Gemini API key is invalid or unauthorized."),
                HttpStatusCode.Forbidden => new InvalidOperationException("Gemini request was forbidden for the current project or key."),
                HttpStatusCode.TooManyRequests => new InvalidOperationException("Gemini rate limit was exceeded."),
                _ => new InvalidOperationException(
                    $"Gemini request failed with status code {(int)statusCode}. Response: {responseBody}")
            };
        }
    }
}