using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public class DatasetUpdate
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string MetadataUuid { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        [Required]
        public string ContactName { get; set; }
        [Required]
        public string OwnerOrganization { get; set; }
        [Required]
        public string RequiredRole { get; set; }
    }
}
