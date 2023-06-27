using Geonorge.OpplastingsApi.Models.Entity;
using System.Text.Json.Serialization;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public class Dataset
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public string? MetadataUuid { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactName { get; set; }
        public string? OwnerOrganization { get; set; }
        public string? RequiredRole { get; set; }
        public List<FileFormat>? AllowedFileFormats { get; set; }
        [JsonIgnore]
        public virtual List<File>? Files { get; set; } = new();
        [JsonPropertyName("files")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<File> SerializationFiles
        {
            get => Files?.Count > 0 ? Files : null;
            set => Files = value ?? new();
        }

        public bool RequireValidFile { get; set; }

    }
}
