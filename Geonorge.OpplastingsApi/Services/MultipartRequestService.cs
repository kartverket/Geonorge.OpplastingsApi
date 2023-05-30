using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Geonorge.OpplastingsApi.Services
{
    public class MultipartRequestService : IMultipartRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MultipartRequestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<InputData> GetFormDataAndFilesFromMultipart()
        {

            var request = _httpContextAccessor.HttpContext.Request;
            var inputData = new InputData();
            var reader = new MultipartReader(request.GetMultipartBoundary(), request.Body);
            MultipartSection section;

            var formAccumulator = new KeyValueAccumulator();

            try
            {
                while ((section = await reader.ReadNextSectionAsync()) != null)
                {

                    if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                    {
                        continue;
                    }

                    if (contentDisposition.IsFormDisposition())
                    {
                        formAccumulator = await AccumulateForm(formAccumulator, section, contentDisposition);
                        inputData.Values = formAccumulator;
                    }
                    else if (contentDisposition.IsFileDisposition()) { 

                    var name = contentDisposition.Name.Value;

                    var fileName =  contentDisposition.FileName;

                    inputData.Files.Add(await CreateFormFile(contentDisposition, section));

                    }

                }

                return inputData;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private static async Task<IFormFile> CreateFormFile(ContentDispositionHeaderValue contentDisposition, MultipartSection section)
        {
            var memoryStream = new MemoryStream();
            await section.Body.CopyToAsync(memoryStream);
            await section.Body.DisposeAsync();
            memoryStream.Position = 0;

            return new FormFile(memoryStream, 0, memoryStream.Length, contentDisposition.Name.ToString(), contentDisposition.FileName.ToString())
            {
                Headers = new HeaderDictionary(),
                ContentType = section.ContentType
            };
        }

        private Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

            // UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }

            return mediaType.Encoding;
        }

        private async Task<KeyValueAccumulator> AccumulateForm(KeyValueAccumulator formAccumulator, MultipartSection section, ContentDispositionHeaderValue contentDisposition)
        {
            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
            using var streamReader = new StreamReader(section.Body, GetEncoding(section), true, 1024, true);
            {
                var value = await streamReader.ReadToEndAsync();
                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                {
                    value = string.Empty;
                }
                formAccumulator.Append(key, value);

                if (formAccumulator.ValueCount > FormReader.DefaultValueCountLimit)
                {
                    throw new InvalidDataException($"Form key count limit {FormReader.DefaultValueCountLimit} exceeded.");
                }
            }

            return formAccumulator;
        }
    }

    public interface IMultipartRequestService
    {
        Task<InputData> GetFormDataAndFilesFromMultipart();
    }
}
