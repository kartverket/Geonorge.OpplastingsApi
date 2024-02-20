namespace Geonorge.OpplastingsApi.Models.Api
{
    public class InputData
    {
        public IFormFile File { get; set; }
        public FileType FileType { get; set; }
        public FileNew FileInfo { get; set; } = new();
        public bool RequireValidFile { get; set; }
    }
}
