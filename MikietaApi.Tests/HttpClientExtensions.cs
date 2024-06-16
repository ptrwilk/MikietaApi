using System.Net;
using System.Text;
using System.Text.Json;

namespace MikietaApi.Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<TResult> As<TResult>(this Task<HttpResponseMessage> response)
        {
            var res = await response.ConfigureAwait(false);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Received status code: {res.StatusCode}");
            }

            var stringContent = await res.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<TResult>(stringContent, options)!;
        }

        public static StringContent ToStringContent(this object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}