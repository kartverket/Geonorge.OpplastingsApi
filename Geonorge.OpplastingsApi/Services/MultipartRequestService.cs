using Geonorge.OpplastingsApi.Models.Api;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Geonorge.OpplastingsApi.Services
{
    public class MultipartRequestService : IMultipartRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MultipartRequestService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<InputFiles> GetFilesFromMultipart()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var reader = new MultipartReader(request.GetMultipartBoundary(), request.Body);
            var inputFiles = new InputFiles();
            MultipartSection section;

            try
            {
                while ((section = await reader.ReadNextSectionAsync()) != null)
                {
                    if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) || !contentDisposition.IsFileDisposition())
                        continue;

                    var name = contentDisposition.Name.Value;

                    var fileName =  contentDisposition.FileName;

                     inputFiles.Files.Add(await CreateFormFile(contentDisposition, section));

                }

                return inputFiles;
            }
            catch
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
    }

    public interface IMultipartRequestService
    {
        Task<InputFiles> GetFilesFromMultipart();
    }
}
