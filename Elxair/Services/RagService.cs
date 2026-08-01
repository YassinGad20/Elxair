using System.Text;
using System.Text.Json;
using Elxair.Models.AI;

namespace Elxair.Services
{
    public class RagService
    {
        private readonly HttpClient _httpClient;

        public RagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskAsync(string message)
        {
            var request = new ChatRequest
            {
                Message = message
            };

            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "http://127.0.0.1:8001/chat",
                content
            );

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var answer = JsonSerializer.Deserialize<ChatResponse>(
                result,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return answer?.Answer ?? "No response.";
        }
    }
}