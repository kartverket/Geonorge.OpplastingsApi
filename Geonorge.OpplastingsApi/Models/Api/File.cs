namespace Geonorge.OpplastingsApi.Models.Api
{
    public class File
    {
        public string FileName { get; set; }
        public virtual Dataset Dataset { get; set; }
    }
}
