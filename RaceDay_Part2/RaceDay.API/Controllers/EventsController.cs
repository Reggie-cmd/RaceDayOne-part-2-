using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/events")]
public class EventsController(RaceDayDbContext db):ControllerBase {
    private int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    /// <summary>Lists events visible to all visitors, including unauthenticated users.</summary>
    [HttpGet,AllowAnonymous] public async Task<IActionResult> List()=>Ok(await db.Events.AsNoTracking().OrderBy(e=>e.EventDate).Select(e=>new {e.EventID,e.EventName,e.Description,e.EventDate,e.StartTime,e.Location,e.DistanceKm,e.EventType,e.Status,e.OrganiserID}).ToListAsync());
    /// <summary>Returns event details and categories.</summary>
    [HttpGet("{id:int}"),AllowAnonymous] public async Task<IActionResult> Get(int id){var e=await db.Events.AsNoTracking().Include(x=>x.Categories).Where(x=>x.EventID==id).Select(x=>new{x.EventID,x.EventName,x.Description,x.EventDate,x.StartTime,x.Location,x.DistanceKm,x.EventType,x.Status,x.OrganiserID,Categories=x.Categories}).FirstOrDefaultAsync();return e is null?NotFound():Ok(e);}
    /// <summary>Creates an event owned by the current organiser.</summary>
    [HttpPost,Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Create(EventRequest r){var error=Validate(r);if(error!=null)return BadRequest(new MessageResponse(error));var e=From(r);e.OrganiserID=UserId;db.Events.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=e.EventID},e);}
    /// <summary>Updates an event only when the current organiser owns it.</summary>
    [HttpPut("{id:int}"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Update(int id,EventRequest r){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganiserID!=UserId)return Forbid();var error=Validate(r);if(error!=null)return BadRequest(new MessageResponse(error));e.EventName=r.EventName;e.Description=r.Description;e.EventDate=r.EventDate;e.StartTime=r.StartTime;e.Location=r.Location;e.DistanceKm=r.DistanceKm;e.EventType=r.EventType;e.Status=r.Status;await db.SaveChangesAsync();return Ok(e);}
    /// <summary>Deletes an event only when the current organiser owns it.</summary>
    [HttpDelete("{id:int}"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> Delete(int id){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(e.OrganiserID!=UserId)return Forbid();db.Events.Remove(e);await db.SaveChangesAsync();return NoContent();}
    private static Event From(EventRequest r)=>new(){EventName=r.EventName,Description=r.Description,EventDate=r.EventDate,StartTime=r.StartTime,Location=r.Location,DistanceKm=r.DistanceKm,EventType=r.EventType,Status=r.Status};
    private static string? Validate(EventRequest r){if(r.DistanceKm<0)return "DistanceKm cannot be negative.";if(r.EventType is not("Running" or "Walking" or "Cycling"))return "EventType must be Running, Walking, or Cycling.";if(r.Status is not("Open" or "Closed" or "Cancelled" or "Completed"))return "Invalid event status.";return null;}
}
