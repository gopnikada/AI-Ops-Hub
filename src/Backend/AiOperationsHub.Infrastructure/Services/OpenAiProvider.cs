using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AiOperationsHub.Application.Abstractions.Providers;
using AiOperationsHub.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Provides a real OpenAI-backed implementation of <see cref="IAiProvider"/>.
    /// </summary>
    public sealed class OpenAiProvider : IAiProvider
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAiOptions _options;
        private readonly ILogger<OpenAiProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAiProvider"/> class.
        /// </summary>
        /// <param name="httpClient">The configured HTTP client.</param>
        /// <param name="options">The OpenAI options.</param>
        /// <param name="logger">The logger.</param>
        public OpenAiProvider(
            HttpClient httpClient,
            IOptions<OpenAiOptions> options,
            ILogger<OpenAiProvider> logger)
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

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "responses");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new
            {
                model = _options.Model,
                input = new object[]
                {
                    new
                    {
                        role = "system",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text = request.SystemPrompt
                            }
                        }
                    },
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text = request.UserPrompt
                            }
                        }
                    }
                }
            };

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenAI request failed. StatusCode: {StatusCode}, CorrelationId: {CorrelationId}, Response: {Response}",
                    (int)response.StatusCode,
                    request.CorrelationId,
                    responseBody);

                throw new InvalidOperationException(
                    $"OpenAI request failed with status code {(int)response.StatusCode}.");
            }

            var outputText = ExtractOutputText(responseBody);

            _logger.LogInformation(
                "OpenAI request completed successfully. CorrelationId: {CorrelationId}, ProviderType: {ProviderType}",
                request.CorrelationId,
                request.ProviderType);

            return new AiProviderResponse
            {
                OutputText = outputText,
                RawResponseJson = responseBody
            };
        }

        /// <summary>
        /// Extracts the primary text output from an OpenAI Responses API payload.
        /// </summary>
        /// <param name="responseBody">The raw JSON response body.</param>
        /// <returns>The extracted text output.</returns>
        private static string ExtractOutputText(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (root.TryGetProperty("output_text", out var outputTextElement) &&
                outputTextElement.ValueKind == JsonValueKind.String)
            {
                var directOutputText = outputTextElement.GetString();

                if (!string.IsNullOrWhiteSpace(directOutputText))
                {
                    return directOutputText;
                }
            }

            if (root.TryGetProperty("output", out var outputElement) &&
                outputElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var outputItem in outputElement.EnumerateArray())
                {
                    if (!outputItem.TryGetProperty("content", out var contentElement) ||
                        contentElement.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    foreach (var contentItem in contentElement.EnumerateArray())
                    {
                        if (contentItem.TryGetProperty("text", out var textElement) &&
                            textElement.ValueKind == JsonValueKind.String)
                        {
                            var text = textElement.GetString();

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                return text;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("OpenAI response did not contain any output text.");
        }
    }
}