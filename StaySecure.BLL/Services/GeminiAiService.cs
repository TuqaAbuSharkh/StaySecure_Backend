using Microsoft.Extensions.Configuration;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services
{
    public class GeminiAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiAiService(
            IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            _apiKey = configuration["Gemini:ApiKey"]!;
        }

        public async Task<string> GenerateFeedbackAsync(string scenarioTitle, bool isCorrect)
        {
            var prompt =
                $"Scenario: {scenarioTitle}\n" +
                $"The user's answer was {(isCorrect ? "correct" : "wrong")}.\n" +
                $"Give educational cybersecurity feedback suitable for {{ageGroup}}.\r\nUse simple language.\r\nMaximum 2 short sentences.";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                requestBody);

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return json;

            using var document = JsonDocument.Parse(json);

            return document
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
                ?? "No feedback generated";
        }


        public async Task<AiScenarioDto?> GenerateScenarioAsync( List<string> weakTopics,AgeGroupEnum ageGroup,LevelEnum level)
        {
            var prompt = $@"
Generate ONE cybersecurity training scenario.

Age Group: {ageGroup}
Level: {level}

Focus on these weak topics:
{string.Join(", ", weakTopics)}

Return ONLY valid JSON.

Example:

{{
  ""title"": ""Suspicious Email"",
  ""description"": ""You receive an email asking for your password."",
  ""category"": ""Phishing"",
  ""options"": [
    {{
      ""text"": ""Send password"",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Verify sender"",
      ""isCorrect"": true
    }},
    {{
      ""text"": ""Ignore antivirus"",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Forward password"",
      ""isCorrect"": false
    }}
  ]
}}
";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new
                    {
                        text = prompt
                    }
                }
            }
        }
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                requestBody);

            var json =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            using var document =
                JsonDocument.Parse(json);

            var generatedText =
                document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

            if (string.IsNullOrWhiteSpace(generatedText))
                return null;

            generatedText = generatedText
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            return JsonSerializer.Deserialize<AiScenarioDto>(
                generatedText,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

    }
}
