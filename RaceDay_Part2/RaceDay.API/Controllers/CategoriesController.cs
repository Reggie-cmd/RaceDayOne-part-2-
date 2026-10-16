using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RaceDay.API.Data;using RaceDay.API.DTOs;using RaceDay.API.Models;
namespace RaceDay.API.Controllers;
[ApiController,Route("api")]
public class CategoriesController(RaceDayDbContext db):ControllerBase {
    private int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    /// <summary>Lists categories for an event.</summary>
    [HttpGet("events/{eventId:int}/categories"),AllowAnonymous] public async Task<IActionResult> List(int eventId){if(!await db.Events.AnyAsync(e=>e.EventID==eventId))return NotFound();return Ok(await db.Categories.Where(c=>c.EventID==eventId).AsNoTracking().ToListAsync());}
    /// <summary>Adds a category to an event owned by the current organiser.</summary>
    [HttpPost("events/{eventId:int}/categories"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Create(int eventId,CategoryRequest r){var e=await db.Events.FindAsync(eventId);if(e is null)return NotFound();if(e.OrganiserID!=UserId)return Forbid();if(r.EntryFee<0||(r.MaxParticipants.HasValue && r.MaxParticipants.Value<0))return BadRequest(new MessageResponse("Entry fee and capacity must not be negative."));var c=new Category{EventID=eventId,CategoryName=r.CategoryName,AgeGroup=r.AgeGroup,EntryFee=r.EntryFee,MaxParticipants=r.MaxParticipants};db.Categories.Add(c);await db.SaveChangesAsync();return Created($"/api/categories/{c.CategoryID}",c);}
    /// <summary>Updates a category only if its parent event belongs to the current organiser.</summary>
    [HttpPut("categories/{id:int}"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Update(int id,CategoryRequest r){var c=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryID==id);if(c is null)return NotFound();if(c.Event.OrganiserID!=UserId)return Forbid();if(r.EntryFee<0||(r.MaxParticipants.HasValue && r.MaxParticipants.Value<0))return BadRequest(new MessageResponse("Entry fee and capacity must not be negative."));c.CategoryName=r.CategoryName;c.AgeGroup=r.AgeGroup;c.EntryFee=r.EntryFee;c.MaxParticipants=r.MaxParticipants;await db.SaveChangesAsync();return Ok(c);}
    /// <summary>Deletes a category only if its parent event belongs to the current organiser.</summary>
    [HttpDelete("categories/{id:int}"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Delete(int id){var c=await db.Categories.Include(x=>x.Event).FirstOrDefaultAsync(x=>x.CategoryID==id);if(c is null)return NotFound();if(c.Event.OrganiserID!=UserId)return Forbid();if(await db.Enrolments.AnyAsync(e=>e.CategoryID==id))return Conflict(new MessageResponse("A category with enrolments cannot be deleted."));db.Categories.Remove(c);await db.SaveChangesAsync();return NoContent();}
}
