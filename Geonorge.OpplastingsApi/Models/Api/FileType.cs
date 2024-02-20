using System.ComponentModel;

namespace Geonorge.OpplastingsApi.Models.Api
{
    public enum FileType
    {
        [Description("application/xml")]
        XML,
        [Description("application/xml+gml")]
        GML32,
        [Description("application/xml")]
        XSD,
        [Description("application/json")]
        JSON,
        [Description("application/geo+json")]
        GeoJSON,
        [Description("application/octet-stream")]
        Unknown
    }
}
