namespace AiOperationsHub.Infrastructure.Chat
{
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using AiOperationsHub.Application.Abstractions.Repositories;
    using AiOperationsHub.Application.Prompts;
    using AiOperationsHub.Application.Tools;
    using AiOperationsHub.Application.Tools.Planning;
    using AiOperationsHub.Infrastructure.Options;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Uses the Gemini API function-calling flow to select one internal tool and its arguments.
    /// </summary>
    public sealed class GeminiToolPlanner : IAiToolPlanner
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly ISystemPromptRepository _systemPromptRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeminiToolPlanner"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="options">The Gemini options.</param>
        /// <param name="systemPromptRepository">The system prompt repository.</param>
        public GeminiToolPlanner(
            HttpClient httpClient,
            IOptions<GeminiOptions> options,
            ISystemPromptRepository systemPromptRepository)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _systemPromptRepository = systemPromptRepository;
        }

        /// <inheritdoc />
        public async Task<ToolPlanningResponse> PlanAsync(
            ToolPlanningRequest request,
            CancellationToken cancellationToken)
        {
            var systemPrompt = await ResolveSystemPromptAsync(cancellationToken);

            var body = new GeminiGenerateContentRequest
            {
                SystemInstruction = new GeminiContent
                {
                    Parts =
                    [
                        new GeminiPart
                        {
                            Text = systemPrompt
                        }
                    ]
                },
                Contents =
                [
                    new GeminiContent
                    {
                        Role = "user",
                        Parts =
                        [
                            new GeminiPart
                            {
                                Text = request.UserMessage
                            }
                        ]
                    }
                ],
                Tools =
                [
                    new GeminiTool
                    {
                        FunctionDeclarations = request.Tools
                            .Select(ToFunctionDeclaration)
                            .ToArray()
                    }
                ],
                ToolConfig = new GeminiToolConfig
                {
                    FunctionCallingConfig = new GeminiFunctionCallingConfig
                    {
                        Mode = "AUTO"
                    }
                }
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"v1beta/models/{Uri.EscapeDataString(_options.Model)}:generateContent?key={Uri.EscapeDataString(_options.ApiKey)}");

            httpRequest.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            httpRequest.Content = new StringContent(
                JsonSerializer.Serialize(body, JsonOptions),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Gemini tool planning call failed with status {(int)response.StatusCode}: {rawResponse}");
            }

            var parsed = JsonSerializer.Deserialize<GeminiGenerateContentResponse>(
                rawResponse,
                JsonOptions);

            var firstCandidate = parsed?.Candidates?.FirstOrDefault();
            var firstFunctionCall = firstCandidate?.Content?.Parts?
                .FirstOrDefault(x => x.FunctionCall is not null)?
                .FunctionCall;

            if (firstFunctionCall is not null)
            {
                return new ToolPlanningResponse
                {
                    Invocation = new ToolInvocation
                    {
                        ToolName = firstFunctionCall.Name,
                        ArgumentsJson = firstFunctionCall.Args.ValueKind == JsonValueKind.Undefined
                            ? "{}"
                            : firstFunctionCall.Args.GetRawText()
                    },
                    RawResponseJson = rawResponse
                };
            }

            var assistantText = string.Concat(
                firstCandidate?.Content?.Parts?
                    .Where(x => !string.IsNullOrWhiteSpace(x.Text))
                    .Select(x => x.Text));

            return new ToolPlanningResponse
            {
                AssistantMessage = string.IsNullOrWhiteSpace(assistantText)
                    ? "I could not determine a suitable action."
                    : assistantText,
                RawResponseJson = rawResponse
            };
        }

        private async Task<string> ResolveSystemPromptAsync(CancellationToken cancellationToken)
        {
            var saved = await _systemPromptRepository.GetByKeyAsync(
                SystemPromptKeys.ChatToolSelection,
                cancellationToken);

            return string.IsNullOrWhiteSpace(saved?.Value)
                ? DefaultSystemPrompts.ChatToolSelection
                : saved.Value;
        }

        private static GeminiFunctionDeclaration ToFunctionDeclaration(ToolDefinition definition)
        {
            using var document = JsonDocument.Parse(definition.InputSchemaJson);

            return new GeminiFunctionDeclaration
            {
                Name = definition.Name,
                Description = definition.Description,
                Parameters = document.RootElement.Clone()
            };
        }

        private sealed class GeminiGenerateContentRequest
        {
            [JsonPropertyName("system_instruction")]
            public GeminiContent SystemInstruction { get; set; } = null!;

            [JsonPropertyName("contents")]
            public GeminiContent[] Contents { get; set; } = Array.Empty<GeminiContent>();

            [JsonPropertyName("tools")]
            public GeminiTool[] Tools { get; set; } = Array.Empty<GeminiTool>();

            [JsonPropertyName("tool_config")]
            public GeminiToolConfig ToolConfig { get; set; } = null!;
        }

        private sealed class GeminiContent
        {
            [JsonPropertyName("role")]
            public string? Role { get; set; }

            [JsonPropertyName("parts")]
            public GeminiPart[] Parts { get; set; } = Array.Empty<GeminiPart>();
        }

        private sealed class GeminiPart
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonPropertyName("functionCall")]
            public GeminiFunctionCall? FunctionCall { get; set; }
        }

        private sealed class GeminiTool
        {
            [JsonPropertyName("function_declarations")]
            public GeminiFunctionDeclaration[] FunctionDeclarations { get; set; } = Array.Empty<GeminiFunctionDeclaration>();
        }

        private sealed class GeminiFunctionDeclaration
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("description")]
            public string Description { get; set; } = null!;

            [JsonPropertyName("parameters")]
            public JsonElement Parameters { get; set; }
        }

        private sealed class GeminiToolConfig
        {
            [JsonPropertyName("function_calling_config")]
            public GeminiFunctionCallingConfig FunctionCallingConfig { get; set; } = null!;
        }

        private sealed class GeminiFunctionCallingConfig
        {
            [JsonPropertyName("mode")]
            public string Mode { get; set; } = "AUTO";
        }

        private sealed class GeminiGenerateContentResponse
        {
            [JsonPropertyName("candidates")]
            public GeminiCandidate[]? Candidates { get; set; }
        }

        private sealed class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent? Content { get; set; }
        }

        private sealed class GeminiFunctionCall
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = null!;

            [JsonPropertyName("args")]
            public JsonElement Args { get; set; }
        }
    }
}