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
            builder.Entity<GroupMember>().HasKey(gu => new { gu.UserId, gu.GroupId });

           // builder.Entity<Flight>()
           //.HasOne(t => t.Airport1)
           //.WithMany()
           //.HasForeignKey(t => t.SourceAirporttId)
           //.OnDelete(DeleteBehavior.Restrict);

           // builder.Entity<Flight>()
           //     .HasOne(t => t.Airport2)
           //     .WithMany()
           //     .HasForeignKey(t => t.DestinationAirporttId)
           //     .OnDelete(DeleteBehavior.Restrict);
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
        public DbSet<GroupDocTypePermission> GroupDocTypePermissions { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<SystemConfigure> SystemConfigures { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
