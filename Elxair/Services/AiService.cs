using System.Net.Http.Json;
using Elxair.Models;

namespace Elxair.Services
{
    public class AiService
    {
        private readonly HttpClient _httpClient;

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetPredictionAsync(PredictionRequest data)
        {
            try
            {
                // تأكد من تغيير الرابط لرابط الـ FastAPI بتاعك
                var response = await _httpClient.PostAsJsonAsync("http://127.0.0.1:8000/predict", data);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                    return result?.PredictedProfit ?? 0;
                }
            }
            catch (Exception ex)
            {
                // سجل الخطأ هنا لو حصل مشكلة في الاتصال
                Console.WriteLine($"Error connecting to AI: {ex.Message}");
            }
            return 0;
        }
    }
}