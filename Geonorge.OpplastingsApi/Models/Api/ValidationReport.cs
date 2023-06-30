namespace Geonorge.OpplastingsApi.Models.Api
{
    public class ValidationReport
    {
        public string CorrelationId { get; set; } = null!;
        public string Namespace { get; set; } = null!;
        public int Errors { get; set; }
        public int Warnings { get; set; }
        public List<ValidationRule> Rules { get; set; } = new();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<string> Files { get; set; } = new();
        public double TimeUsed => Math.Round(EndTime.Subtract(StartTime).TotalSeconds, 2);
    }

    public class ValidationRule
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<Dictionary<string, object>> Messages { get; set; } = new();
        public string Status { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Documentation { get; set; } = null!;
        public string MessageType { get; set; } = null!;
    }
}
