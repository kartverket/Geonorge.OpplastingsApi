using System.ComponentModel.DataAnnotations;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public class FileNew
    {
        /// <summary>
        /// Den autogenererte iden for filen
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        /// <summary>
        /// Bruker sin opplastet filnavn
        /// </summary>
        /// <example>gravplass.gml</example>
        public string FileName { get; set; }
        /// <summary>
        /// Default status Sendt inn
        /// </summary>
        /// <example>Sendt inn</example>
        public string Status { get; set; }
        /// <summary>
        /// Referanse til datasett id
        /// </summary>
        /// <example>1</example>
        [Required]
        public int datasetId { get; set; }
    }
}
