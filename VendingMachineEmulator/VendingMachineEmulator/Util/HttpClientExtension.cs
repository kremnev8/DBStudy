using System.Net.Http;
using System.Threading.Tasks;
using AsyncBridge;
using Newtonsoft.Json;

namespace VendingMachineEmulator.Util
{
    public static class HttpClientExtension
    {
        public static async Task<T> GetJsonAsync<T>(this HttpClient client, string url)
        {
            string json = await GetAsync(client, url);
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }
        
        public static async Task<T> SendAsyncAsJson<T>(this HttpClient client, HttpRequestMessage request)
        {
            string json = await SendAsync(client, request);
            T result = JsonConvert.DeserializeObject<T>(json);
            return result;
        }

        private static async Task<string> GetAsync(HttpClient client, string url)
        {
            HttpResponseMessage message =  await client.GetAsync(url);
            message.EnsureSuccessStatusCode();

            return await message.Content.ReadAsStringAsync();
        }
        
        private static async Task<string> SendAsync(HttpClient client, HttpRequestMessage request)
        {
            HttpResponseMessage message =  await client.SendAsync(request);
            message.EnsureSuccessStatusCode();

            return await message.Content.ReadAsStringAsync();
        }
        
        
    }
}