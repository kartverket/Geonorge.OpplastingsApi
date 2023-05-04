using Microsoft.EntityFrameworkCore;

namespace Geonorge.OpplastingsApi.Models.Entity
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
                : base(options) {
                }

        public DbSet<Dataset> Datasets { get; set; }
    }
}
