using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace Cards.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApiUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            
            base.OnModelCreating(modelBuilder);

            // Create one to one relationship between card and status
            modelBuilder.Entity<Status>()
                .HasMany(x => x.Cards)
                .WithOne(x => x.Status)
                .HasForeignKey(x => x.StatusId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Create One to Many rshp between User and cards
            modelBuilder.Entity<ApiUser>()
                .HasMany(x => x.Cards)
                .WithOne(y => y.User)
                .HasForeignKey(x => x.CreatedBy)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Initial database seeder.
            modelBuilder.Entity<Status>().HasData(
                new { Id = 1, Name = "To Do", CreatedDate = DateTime.UtcNow, LastModifiedDate = DateTime.UtcNow },
                new { Id = 2, Name = "In progress", CreatedDate = DateTime.UtcNow, LastModifiedDate = DateTime.UtcNow },
                new { Id = 3, Name = "Done", CreatedDate = DateTime.UtcNow, LastModifiedDate = DateTime.UtcNow });

            this.SeedUsers(modelBuilder);
            this.SeedRoles(modelBuilder);
            this.SeedUserRoles(modelBuilder);
        }

        private void SeedUsers(ModelBuilder builder)
        {
            ApiUser admin = new ApiUser()
            {
                Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                UserName = "Admin",
                NormalizedUserName = "Admin",
                Email = "admin@test.com",
                NormalizedEmail = "admin@test.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567890"
            };

            PasswordHasher<ApiUser> passwordHasher = new PasswordHasher<ApiUser>();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin*123");

            ApiUser member = new ApiUser()
            {
                Id = "a319d974-6dd4-4c0e-8627-b73e82ed8457",
                UserName = "Member",
                NormalizedUserName = "Member",
                Email = "member@test.com",
                NormalizedEmail = "member@test.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567891"
            };

            PasswordHasher<ApiUser> passwordHasherMember = new PasswordHasher<ApiUser>();
            member.PasswordHash = passwordHasherMember.HashPassword(member, "Member*123");

            ApiUser member1 = new ApiUser()
            {
                Id = "b319d974-6dd4-4c0e-8627-b73e82ed8458",
                UserName = "Member1",
                NormalizedUserName = "Member1",
                Email = "anothermember@test.com",
                NormalizedEmail = "anothermember@test.com",
                LockoutEnabled = false,
                PhoneNumber = "1234567892"
            };

            member1.PasswordHash = passwordHasherMember.HashPassword(member1, "Member*123");

            builder.Entity<ApiUser>().HasData(admin, member, member1);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "fab4fac1-c546-41de-aebc-a14da6895711", Name = "Admin", ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "Admin" },
                new IdentityRole() { Id = "c7b013f0-5201-4317-abd8-c211f91b7330", Name = "Member", ConcurrencyStamp = Guid.NewGuid().ToString(), NormalizedName = "Member" }
                );
        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>() { RoleId = "fab4fac1-c546-41de-aebc-a14da6895711", UserId = "b74ddd14-6340-4840-95c2-db12554843e5" },
                new IdentityUserRole<string>() { RoleId = "c7b013f0-5201-4317-abd8-c211f91b7330", UserId = "a319d974-6dd4-4c0e-8627-b73e82ed8457" },
                new IdentityUserRole<string>() { RoleId = "c7b013f0-5201-4317-abd8-c211f91b7330", UserId = "b319d974-6dd4-4c0e-8627-b73e82ed8458" }

                );
        }

        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Status> Statuses => Set<Status>();
    }
}
