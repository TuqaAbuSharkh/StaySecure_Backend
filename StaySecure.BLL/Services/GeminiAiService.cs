using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response;
using StaySecure.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Numerics;
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

        public async Task<string?> GenerateDailyTipAsync(string category, string lang)
        {
            var prompt = $@"
Generate ONE cybersecurity awareness tip.

Weak category: {category}

Language: {lang}

Requirements:
- Maximum 25 words.
- Easy language.
- Practical advice.
- If Language is Arabic, write in Modern Standard Arabic.
- If Language is English, write in English.
- Return ONLY the tip text.
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

            return document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();
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

Requirements:
- Generate one realistic cybersecurity scenario.
- Include a short hint that helps the user think without revealing the correct answer.
- Return exactly FOUR answer choices.
- Only ONE answer is correct.
- Return ONLY valid JSON.

Example:

{{
  ""title"": ""Suspicious Email"",
  ""description"": ""You receive an email asking for your password."",
  ""category"": ""Phishing"",
  ""hint"": ""Verify the sender before taking any action."",
  ""options"": [
    {{
      ""text"": ""Send your password"",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Verify the sender before responding"",
      ""isCorrect"": true
    }},
    {{
      ""text"": ""Disable antivirus"",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Forward your password"",
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


        public async Task<AiChallengeDto?> GenerateChallengeAsync()
        {
            var prompt = $@"
Random Seed: {Guid.NewGuid()}

Generate ONE unique AI cybersecurity challenge for a cybersecurity awareness platform.

Audience:
University students.

Difficulty:
Advanced Cybersecurity Awareness Challenge.

IMPORTANT:
The challenge should be practical, realistic, engaging, and easy to read.

Rules:

- Maximum 45 words.
- Maximum 2 short sentences.
- Reading time should be less than 20 seconds.
- Generate a DIFFERENT challenge every time.
- Avoid repeating previous ideas.
- Avoid overly technical or enterprise-level scenarios.
- Do NOT generate SOC investigations, SIEM alerts, malware analysis reports, digital forensics, penetration testing tasks, or incident response reports.
- Do NOT require programming or technical tools.
- Focus on cybersecurity awareness and decision making.

Randomly choose ONE topic:

• Phishing
• Social Engineering
• Password Security
• Authentication
• Public Wi-Fi
• Safe Browsing
• Malware
• USB Attacks
• QR Code Scams
• Online Privacy
• Multi-Factor Authentication
• Fake Websites
• Email Security
• Mobile Security

Randomly choose ONE challenge style:

- Best decision
- Safest action
- Biggest red flag
- Most secure behavior
- Best prevention method

Generate a short catchy title (maximum 4 words).

Generate ONE short hint (maximum 6 words).

Generate EXACTLY four answer choices.

Only ONE answer must be correct.

Return ONLY valid JSON.

Example:

{{
  ""title"": ""Fake QR Code"",
  ""description"": ""You scan a QR code in a café that immediately asks for your banking login. What is the safest action?"",
  ""category"": ""QR Code Scams"",
  ""hint"": ""Verify before trusting."",
  ""options"": [
    {{
      ""text"": ""Enter your banking credentials."",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Close the page and use the bank's official app instead."",
      ""isCorrect"": true
    }},
    {{
      ""text"": ""Share the QR code with friends."",
      ""isCorrect"": false
    }},
    {{
      ""text"": ""Disable your phone security settings."",
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

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var generatedText = document.RootElement
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

            try
            {
                var challenge = JsonSerializer.Deserialize<AiChallengeDto>(
                    generatedText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (challenge == null ||
                    challenge.Options == null ||
                    challenge.Options.Count != 4)
                    return null;

                return challenge;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GenerateChallengeFeedbackAsync(
            string title,
            string description,
            bool isCorrect)
        {
            var prompt = $@"
Challenge:

Title:
{title}

Description:
{description}

The student's answer was {(isCorrect ? "correct" : "incorrect")}.

Generate educational cybersecurity feedback.

Requirements:

- Maximum 40 words.
- Friendly.
- Explain briefly why the answer is {(isCorrect ? "correct" : "incorrect")}.
- Give one cybersecurity recommendation.
- Return ONLY the feedback.
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
    }
}
