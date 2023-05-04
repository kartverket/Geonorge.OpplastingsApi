using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class Dataset
    {
        public int Id { get; set; }
        [StringLength(255)]
        [Required]
        public string Title { get; set; }
    }
}
