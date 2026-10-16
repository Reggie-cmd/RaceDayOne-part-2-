using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
namespace RaceDay.API.Data;
public class RaceDayDbContext(DbContextOptions<RaceDayDbContext> options) : DbContext(options) {
    public DbSet<User> Users => Set<User>(); public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Event> Events => Set<Event>(); public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>(); public DbSet<Result> Results => Set<Result>();
    protected override void OnModelCreating(ModelBuilder b) {
        b.Entity<User>().ToTable("Users", t => t.HasCheckConstraint("CK_Users_Role", "[Role] IN ('Organiser','Participant')"));
        b.Entity<User>().HasKey(x => x.UserID); b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<User>().Property(x => x.Email).HasMaxLength(256); b.Entity<User>().Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Entity<UserProfile>().ToTable("UserProfiles"); b.Entity<UserProfile>().HasKey(x => x.UserProfileID); b.Entity<UserProfile>().HasIndex(x => x.UserID).IsUnique();
        b.Entity<UserProfile>().HasOne(x => x.User).WithOne(x => x.UserProfile).HasForeignKey<UserProfile>(x => x.UserID).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Event>().ToTable("Events", t => { t.HasCheckConstraint("CK_Events_EventType", "[EventType] IN ('Running','Walking','Cycling')"); t.HasCheckConstraint("CK_Events_Status", "[Status] IN ('Open','Closed','Cancelled','Completed')"); });
        b.Entity<Event>().HasKey(x => x.EventID); b.Entity<Event>().Property(x => x.DistanceKm).HasPrecision(8,2);
        b.Entity<Event>().HasOne(x => x.Organiser).WithMany(x => x.OrganisedEvents).HasForeignKey(x => x.OrganiserID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Category>().ToTable("Categories"); b.Entity<Category>().HasKey(x => x.CategoryID); b.Entity<Category>().Property(x => x.EntryFee).HasPrecision(10,2);
        b.Entity<Category>().HasOne(x => x.Event).WithMany(x => x.Categories).HasForeignKey(x => x.EventID).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Enrolment>().ToTable("Enrolments"); b.Entity<Enrolment>().HasKey(x => x.EnrolmentID); b.Entity<Enrolment>().HasIndex(x => new { x.ParticipantID, x.CategoryID }).IsUnique();
        b.Entity<Enrolment>().HasOne(x => x.Participant).WithMany(x => x.Enrolments).HasForeignKey(x => x.ParticipantID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Enrolment>().HasOne(x => x.Category).WithMany(x => x.Enrolments).HasForeignKey(x => x.CategoryID).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Result>().ToTable("Results"); b.Entity<Result>().HasKey(x => x.ResultID); b.Entity<Result>().HasIndex(x => x.EnrolmentID).IsUnique();
        b.Entity<Result>().HasOne(x => x.Enrolment).WithOne(x => x.Result).HasForeignKey<Result>(x => x.EnrolmentID).OnDelete(DeleteBehavior.Cascade);
    }
}
