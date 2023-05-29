using FileDocument.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FileDocument.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GroupUser>().HasKey(gu => new { gu.UserId, gu.GroupId });
            base.OnModelCreating(builder);
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<Airport> Airports { get; set; }
        public DbSet<User> ApplicationUsers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentRevision> DocumentRevisions { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupDocPermission> GroupDocPermissions { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<SystemConfigure> SystemConfigures { get; set; }
    }
}
