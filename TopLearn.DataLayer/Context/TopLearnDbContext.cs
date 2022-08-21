using Microsoft.EntityFrameworkCore;
using TopLearn.DataLayer.Entities.Course;
using TopLearn.DataLayer.Entities.Order;
using TopLearn.DataLayer.Entities.Permission;
using TopLearn.DataLayer.Entities.User;
using TopLearn.DataLayer.Entities.Wallet;

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
        public DbSet<UserDiscountCode> UserDiscountCodes { get; set; }

        #endregion

        #region Wallet

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletType> WalletTypes { get; set; }

        #endregion

        #region Permission

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        #region Course

        public DbSet<CourseGroup> CourseGroups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseLevel> CourseLevels { get; set; }
        public DbSet<CourseEpisode> CourseEpisodes { get; set; }
        public DbSet<CourseStatus> CourseStatus { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }
        public DbSet<CourseComment> CourseComments { get; set; }
        public DbSet<CourseVote> CourseVotes { get; set; }

        #endregion

        #region Order

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(c => c.HasQueryFilter(u => !u.IsRemoved));
            modelBuilder.Entity<Role>(c => c.HasQueryFilter(r => !r.IsRemoved));
            modelBuilder.Entity<CourseGroup>(c => c.HasQueryFilter(g => !g.IsRemoved));
            modelBuilder.Entity<Course>(c => c.HasQueryFilter(c => !c.IsRemoved));
            modelBuilder.Entity<CourseComment>(c => c.HasQueryFilter(c => !c.IsRemoved));

            base.OnModelCreating(modelBuilder);
        }
    }
}