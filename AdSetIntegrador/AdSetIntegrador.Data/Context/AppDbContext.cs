using Microsoft.EntityFrameworkCore;

namespace AdSetIntegrador.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
