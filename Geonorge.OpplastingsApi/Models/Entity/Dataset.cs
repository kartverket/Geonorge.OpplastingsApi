using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class Dataset
    {
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string MetadataUuid { get; set; }
        [StringLength(255)]
        [Required]
        public string Title { get; set; }
        [StringLength(255)]
        [Required]
        public string OwnerOrganization { get; set; }
        [StringLength(255)]
        [Required]
        public string ContactName { get; set; }
        [StringLength(255)]
        [Required]
        public string ContactEmail { get; set; }
        [StringLength(255)]
        [Required]
        public string RequiredRole { get; set; }

        public virtual List<File>? Files { get; set; }

        public virtual List<FileFormat>? AllowedFileFormats { get; set; }
    }
}
