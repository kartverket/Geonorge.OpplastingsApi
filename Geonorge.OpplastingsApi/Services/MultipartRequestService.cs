using Geonorge.OpplastingsApi.Models.Api;
using Geonorge.OpplastingsApi.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace Geonorge.OpplastingsApi.Services
{
    public class MultipartRequestService : IMultipartRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MultipartRequestService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<InputData> GetInputDataFromMultipartAsync()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var reader = new MultipartReader(request.GetMultipartBoundary(), request.Body);
            var formAccumulator = new KeyValueAccumulator();
            IFormFile file = null;
            FileType fileType = FileType.Unknown;
            MultipartSection section;

            try
            {
                while ((section = await reader.ReadNextSectionAsync()) != null)
                {
                    if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                        continue;

                    var name = contentDisposition.Name.Value;

                    if (contentDisposition.IsFileDisposition() && name == "file" && file == null)
                    {
                        fileType = await FileHelper.GetFileTypeAsync(section);
                        file = await CreateFormFileAsync(contentDisposition, section);
                    }
                    else if (contentDisposition.IsFormDisposition() && (name == "datasetId" || name == "requireValidFile"))
                    {
                        formAccumulator = await AccumulateFormAsync(formAccumulator, section, contentDisposition);
                    }
                }

                return new InputData
                {
                    File = file,
                    FileType = fileType,
                    FileInfo = new FileNew
                    {
                        datasetId = GetDatasetId(formAccumulator)
                    },
                    RequireValidFile = GetRequireValidFiles(formAccumulator),
                };
            }
            catch
            {
                return null;
            }
        }

        private static async Task<IFormFile> CreateFormFileAsync(ContentDispositionHeaderValue contentDisposition, MultipartSection section)
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

        private static async Task<KeyValueAccumulator> AccumulateFormAsync(
            KeyValueAccumulator formAccumulator, MultipartSection section, ContentDispositionHeaderValue contentDisposition)
        {
            var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;

            using var streamReader = new StreamReader(section.Body, GetEncoding(section), true, 1024, true);
            {
                var value = await streamReader.ReadToEndAsync();

                if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
                    value = string.Empty;

                formAccumulator.Append(key, value);

                if (formAccumulator.ValueCount > FormReader.DefaultValueCountLimit)
                    throw new InvalidDataException($"Form key count limit {FormReader.DefaultValueCountLimit} exceeded.");
            }

            return formAccumulator;
        }

        private static Encoding GetEncoding(MultipartSection section)
        {
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);

#pragma warning disable SYSLIB0001
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
                return Encoding.UTF8;
#pragma warning restore SYSLIB0001

            return mediaType.Encoding;
        }

        private static int GetDatasetId(KeyValueAccumulator formAccumulator)
        {
            var accumulatedValues = formAccumulator.GetResults();
            accumulatedValues.TryGetValue("datasetId", out var value);

            if (!int.TryParse(value.ToString(), out var datasetId))
                throw new Exception($"Dataset-ID {value} er ikke gyldig");

            return datasetId;
        }

        private static bool GetRequireValidFiles(KeyValueAccumulator formAccumulator)
        {
            var accumulatedValues = formAccumulator.GetResults();
            accumulatedValues.TryGetValue("requireValidFile", out var value);
            var requireValidString = value.ToString();

            if (string.IsNullOrWhiteSpace(requireValidString))
                return false;

            return bool.TryParse(requireValidString, out var requireValid) && requireValid;
        }
    }

    public interface IMultipartRequestService
    {
        Task<InputData> GetInputDataFromMultipartAsync();
    }
}
