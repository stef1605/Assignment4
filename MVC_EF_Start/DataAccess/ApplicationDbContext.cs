using Microsoft.EntityFrameworkCore;
using MVC_EF_Start.Models;

namespace MVC_EF_Start.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<CommitteeDT> CommitteeDT { get; set; }
        public DbSet<Party> Party { get; set; }
        public DbSet<Funding> Funding { get; set; }
        public DbSet<CandFunding> CandFunding { get; set; }
        public DbSet<Location> Location { get; set; }
    }
}