using Microsoft.EntityFrameworkCore;

namespace SAM.DI
{
    public class LLCContext : DbContext
    {
        //public DbSet<Blog> Blogs { get; set; }
        //public DbSet<Post> Posts { get; set; }

        public LLCContext(DbContextOptions<LLCContext> options) : base(options)
        {

        }
    }
}
