using StaySecure.BLL.Services.IServices;
using StaySecure.DAL.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StaySecure.BLL.Services
{
    //public class AiService : IAiService
    //{
    //    private readonly HttpClient _httpClient;

    //    public AiService(HttpClient httpClient)
    //    {
    //        _httpClient = httpClient;
    //    }


    //    public async Task<string> GenerateFeedbackAsync(
    //  string scenarioTitle,
    //  bool isCorrect)
    //    {
    //        using var client = new HttpClient();

    //        var response = await client.PostAsJsonAsync(
    //            "http://127.0.0.1:11434/api/generate",
    //            new
    //            {
    //                model = "gemma3:1b",
    //                prompt = $"Scenario: {scenarioTitle}. User answer is {(isCorrect ? "correct" : "wrong")}. Give educational cybersecurity feedback in 2 short sentences.",
    //                stream = false
    //            });

    //        return await response.Content.ReadAsStringAsync();
    //    }

    //}
}
