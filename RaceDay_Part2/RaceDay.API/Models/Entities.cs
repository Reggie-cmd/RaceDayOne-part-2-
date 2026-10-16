using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Models;
public class User {
    public int UserID { get; set; }
    [Required, MaxLength(80)] public string FirstName { get; set; } = "";
    [Required, MaxLength(80)] public string LastName { get; set; } = "";
    [Required, MaxLength(256)] public string Email { get; set; } = "";
    [Required] public string PasswordHash { get; set; } = "";
    [Required, MaxLength(20)] public string Role { get; set; } = "Participant";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserProfile? UserProfile { get; set; }
    public ICollection<Event> OrganisedEvents { get; set; } = new List<Event>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
public class UserProfile {
    public int UserProfileID { get; set; } public int UserID { get; set; }
    [MaxLength(30)] public string? PhoneNumber { get; set; } [MaxLength(250)] public string? Address { get; set; }
    [MaxLength(120)] public string? EmergencyContactName { get; set; } [MaxLength(30)] public string? EmergencyContactPhone { get; set; }
    public User User { get; set; } = null!;
}
public class Event {
    public int EventID { get; set; } public int OrganiserID { get; set; }
    [Required, MaxLength(150)] public string EventName { get; set; } = ""; [MaxLength(2000)] public string? Description { get; set; }
    public DateTime EventDate { get; set; } public TimeSpan StartTime { get; set; }
    [Required, MaxLength(250)] public string Location { get; set; } = ""; public decimal DistanceKm { get; set; }
    [Required, MaxLength(20)] public string EventType { get; set; } = "Running";
    [Required, MaxLength(20)] public string Status { get; set; } = "Open"; public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User Organiser { get; set; } = null!; public ICollection<Category> Categories { get; set; } = new List<Category>();
}
public class Category {
    public int CategoryID { get; set; } public int EventID { get; set; }
    [Required, MaxLength(100)] public string CategoryName { get; set; } = ""; [MaxLength(50)] public string? AgeGroup { get; set; }
    public decimal EntryFee { get; set; } public int? MaxParticipants { get; set; }
    public Event Event { get; set; } = null!; public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
public class Enrolment {
    public int EnrolmentID { get; set; } public int ParticipantID { get; set; } public int CategoryID { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow; [Required, MaxLength(20)] public string Status { get; set; } = "Confirmed";
    [MaxLength(30)] public string? RaceNumber { get; set; } public User Participant { get; set; } = null!;
    public Category Category { get; set; } = null!; public Result? Result { get; set; }
}
public class Result {
    public int ResultID { get; set; } public int EnrolmentID { get; set; } public int? FinishPosition { get; set; }
    public TimeSpan? FinishTime { get; set; } [Required, MaxLength(30)] public string ResultStatus { get; set; } = "Finished";
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow; public Enrolment Enrolment { get; set; } = null!;
}
