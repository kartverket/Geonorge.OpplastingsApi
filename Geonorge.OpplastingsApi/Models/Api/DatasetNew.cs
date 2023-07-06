﻿using Geonorge.OpplastingsApi.Models.Entity;
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
        /// <summary>
        /// Epost med informasjon om opplastet fil blir sendt til kontaktperson for datasettet
        /// </summary>
        /// <example>epost@epost.no</example>
        [Required]
        [EmailAddress]
        public string ContactEmail { get; set; }
        /// <summary>
        /// Ekstra epost med informasjon om opplastet fil, kan være til felles epost eller at annen person også jobber med datasettet
        /// </summary>
        /// <example>firmapost@epost.no</example>
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string? ContactEmailExtra { get; set; }
        /// <summary>
        /// Fullt navn på kontaktpersonen
        /// </summary>
        /// <example>Ola Nordmann</example>
        [Required]
        public string ContactName { get; set; }
        /// <summary>
        /// Organisasjonsnavn i metadata/registeret https://register.geonorge.no/organisasjoner som eier datasettet
        /// </summary>
        /// <example>Kartverket</example>
        [Required]
        public string OwnerOrganization { get; set; }
        /// <summary>
        /// Rolle i Baat som brukeren må ha for å få tilgang til datasettet
        /// </summary>
        /// <example>nd.gjenbruk_gravplass</example>
        [Required]
        public string RequiredRole { get; set; }
        /// <summary>
        /// Lovlige filformater (filendelser)
        /// </summary>
        /// <example>[&quot;gml&quot;]</example>
        [Required]
        public List<string>? AllowedFileFormats { get; set; }
        /// <summary>
        /// Kreve at fil må være gyldig for å få legge inn
        /// </summary>
        /// <example>true</example>
        public bool RequireValidFile { get; set; }
    }
}
