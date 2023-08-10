using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
                : base(options) {
                }

        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<FileFormat> FileFormats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {    
            modelBuilder.Entity<FileFormat>()
                .HasData(
                    new FileFormat
                    {
                       Extension = "gml",
                       Name = "Geography Markup Language"   
                    },
                    new FileFormat
                    {
                        Extension = "sos",
                        Name = "Samordnet Opplegg for Stedfestet Informasjon (SOSI)"
                    },
                    new FileFormat
                    {
                        Extension = "gdb",
                        Name = "ESRI file Geodatabase"
                    },
                    new FileFormat
                    {
                        Extension = "shp",
                        Name = "Shape"
                    },
                    new FileFormat
                    {
                        Extension = "gpkg",
                        Name = "GeoPackage"
                    }
            );

            modelBuilder.Entity<Dataset>()
            .HasMany(e => e.AllowedFileFormats)
            .WithMany(e => e.Datasets)
            .UsingEntity("DatasetAllowedFileFormats");
        }
    }
}
