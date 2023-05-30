using Microsoft.AspNetCore.WebUtilities;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public class InputData
    {
        public KeyValueAccumulator Values { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }
}
