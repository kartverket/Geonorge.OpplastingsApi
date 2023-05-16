﻿namespace Geonorge.OpplastingsApi.Models.Api
{
    public class DatasetUpdate
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string MetadataUuid { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string OwnerOrganization { get; set; }
        public string RequiredRole { get; set; }
    }
}
