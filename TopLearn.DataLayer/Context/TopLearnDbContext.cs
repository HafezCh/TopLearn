using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.DataLayer.Context
{
    public class TopLearnDbContext : DbContext
    {
        public TopLearnDbContext(DbContextOptions<TopLearnDbContext> options) : base(options)
        {
        }

        #region User

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion
    }
}