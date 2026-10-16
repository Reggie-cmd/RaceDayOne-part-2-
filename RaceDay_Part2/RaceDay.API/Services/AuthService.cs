using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Services;
public class AuthService(RaceDayDbContext db,PasswordService passwords,IConfiguration config) {
    public async Task<(User? user,string? error)> Register(RegisterRequest request) {
        if (await db.Users.AnyAsync(u => u.Email == request.Email.Trim().ToLower())) return (null,"An account with this email already exists.");
        var role=request.Role.Trim();
        if (role is not ("Participant" or "Organiser")) return (null,"Role must be Participant or Organiser.");
        if (role=="Organiser" && (string.IsNullOrWhiteSpace(request.OrganiserCode) || request.OrganiserCode != config["OrganiserRegistrationCode"])) return (null,"A valid organiser registration code is required.");
        var user=new User { FirstName=request.FirstName.Trim(),LastName=request.LastName.Trim(),Email=request.Email.Trim().ToLowerInvariant(),Role=role };
        user.PasswordHash=passwords.Hash(user,request.Password); db.Users.Add(user); await db.SaveChangesAsync();
        return (user,null);
    }
    public async Task<User?> Login(string email,string password) {
        var user=await db.Users.SingleOrDefaultAsync(u=>u.Email==email.Trim().ToLowerInvariant());
        return user is not null && passwords.Verify(user,password) ? user : null;
    }
}
