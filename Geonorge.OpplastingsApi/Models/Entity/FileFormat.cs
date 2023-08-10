using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class FileFormat
    {
        [Key]
        [StringLength(255)]
        public string Extension { get; set; }
        [StringLength(255)]
        public string Name { get; set; }

        public virtual List<Dataset> Datasets { get; set; }
    }
}
