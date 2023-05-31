using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public class DatasetNew
    {
        /// <summary>
        /// Dataset/metadata tittel
        /// </summary>
        /// <example>Gravplass</example>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// Metadata uuid
        /// </summary>
        /// <example>261a2a6a-bcae-43bd-b7c3-cde40b22ee55</example>
        [Required]
        public string MetadataUuid { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string OwnerOrganization { get; set; }
        public string RequiredRole { get; set; }
    }
}
