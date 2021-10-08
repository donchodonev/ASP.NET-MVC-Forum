namespace ASP.NET_MVC_Blog.Data
{
    using ASP.NET_MVC_Blog.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> BaseUsers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
/*
            builder.Entity<User>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<User>(u => u.IdenityUserId)
                .OnDelete(DeleteBehavior.Restrict);*/
        }
    }
}
