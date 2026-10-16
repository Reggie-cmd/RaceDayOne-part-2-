using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/users"),Authorize]
public class UsersController(RaceDayDbContext db):ControllerBase {
    private int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    /// <summary>Returns the authenticated user's account and profile.</summary>
    [HttpGet("me")] public async Task<IActionResult> Me(){var u=await db.Users.Include(x=>x.UserProfile).FirstAsync(x=>x.UserID==UserId);return Ok(new {u.UserID,u.FirstName,u.LastName,u.Email,u.Role,u.CreatedAt,Profile=u.UserProfile});}
    /// <summary>Updates the authenticated user's profile fields.</summary>
    [HttpPut("me")] public async Task<IActionResult> Update(ProfileUpdateRequest request){
        var profile=await db.UserProfiles.SingleOrDefaultAsync(x=>x.UserID==UserId);
        if(profile is null){profile=new UserProfile{UserID=UserId};db.UserProfiles.Add(profile);}
        profile.PhoneNumber=request.PhoneNumber;profile.Address=request.Address;profile.EmergencyContactName=request.EmergencyContactName;profile.EmergencyContactPhone=request.EmergencyContactPhone;
        await db.SaveChangesAsync();return Ok(profile);
    }
}
