using Microsoft.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Data
{
    public class PMSDBContext : DbContext
    {
        public PMSDBContext(DbContextOptions<PMSDBContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; }
    }
}