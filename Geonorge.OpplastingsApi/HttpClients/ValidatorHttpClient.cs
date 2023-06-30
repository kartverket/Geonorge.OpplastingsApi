using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace Geonorge.OpplastingsApi.HttpClients
{
    public class ValidatorHttpClient : IValidatorHttpClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<ValidatorConfiguration> _options;
        private readonly ILogger<ValidatorHttpClient> _logger;

        public ValidatorHttpClient(
            HttpClient client,
            IOptions<ValidatorConfiguration> options,
            ILogger<ValidatorHttpClient> logger)
        {
            _client = client;
            _options = options;
            _logger = logger;
        }

        public async Task<ValidationReport?> ValidateAsync(IFormFile file)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, _options.Value.ApiUrl)
            {
                Content = CreateFormData(file)
            };

            using var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP Status Code {StatusCode}: Kunne ikke validere '{FileName}'", response.StatusCode, file.FileName);
                throw new Exception($"Kunne ikke validere '{file.FileName}' ({response.StatusCode})");
            }

            return await response.Content.ReadFromJsonAsync<ValidationReport>();
        }

        private static MultipartFormDataContent CreateFormData(IFormFile file)
        {
            var formData = new MultipartFormDataContent
            {
                { CreateStreamContent(file.OpenReadStream(), file.ContentType), "files", file.FileName },
            };

            if (formData.Headers.ContentType != null)
                formData.Headers.ContentType.MediaType = "multipart/form-data";

            return formData;
        }

        private static StreamContent CreateStreamContent(Stream stream, string mimeType)
        {
            var streamContent = new StreamContent(stream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            return streamContent;
        }
    }
}
