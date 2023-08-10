using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class File
    {
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string FileName { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [StringLength(255)]
        [Required]
        public string UploaderPerson { get; set; }
        [StringLength(255)]
        [Required]
        public string UploaderOrganization { get; set; }
        [StringLength(255)]
        [Required]
        public string UploaderEmail { get; set; }
        [StringLength(255)]
        [Required]
        public string UploaderUsername { get; set; }
        [StringLength(255)]
        [Required]
        public string Status { get; set; }

        public virtual Dataset Dataset { get; set; }
    }
}
