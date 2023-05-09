namespace Geonorge.OpplastingsApi.Models.Api
{
    public class Dataset
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual List<File>? Files { get; set; } 
    }
}
