using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.API.Data;using RaceDay.API.DTOs;using RaceDay.API.Services;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/enrolments"),Authorize]
public class EnrolmentsController(RaceDayDbContext db,EnrolmentService service):ControllerBase {
 private int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 /// <summary>Enrols the authenticated participant in a category.</summary>
 [HttpPost,Authorize(Policy="ParticipantOnly")] public async Task<IActionResult> Create(EnrolmentRequest r){var (item,error)=await service.Create(UserId,r.CategoryID);if(item is null)return BadRequest(new MessageResponse(error!));return Created($"/api/enrolments/{item.EnrolmentID}",item);}
 /// <summary>Lists only the current participant's enrolments.</summary>
 [HttpGet("me"),Authorize(Policy="ParticipantOnly")] public async Task<IActionResult> Mine()=>Ok(await db.Enrolments.Where(e=>e.ParticipantID==UserId).Include(e=>e.Category).ThenInclude(c=>c.Event).Include(e=>e.Result).AsNoTracking().ToListAsync());
 /// <summary>Lists enrolments for an event owned by the authenticated organiser.</summary>
 [HttpGet("events/{eventId:int}"),Authorize(Policy="OrganiserOnly")] public async Task<IActionResult> ForEvent(int eventId){var e=await db.Events.FindAsync(eventId);if(e is null)return NotFound();if(e.OrganiserID!=UserId)return Forbid();return Ok(await db.Enrolments.Where(x=>x.Category.EventID==eventId).Include(x=>x.Participant).Include(x=>x.Category).AsNoTracking().Select(x=>new{x.EnrolmentID,x.ParticipantID,ParticipantName=x.Participant.FirstName+" "+x.Participant.LastName,x.CategoryID,CategoryName=x.Category.CategoryName,x.EnrolmentDate,x.Status,x.RaceNumber}).ToListAsync());}
 /// <summary>Cancels the current participant's enrolment.</summary>
 [HttpDelete("{id:int}"),Authorize(Policy="ParticipantOnly")] public async Task<IActionResult> Cancel(int id){var e=await db.Enrolments.FindAsync(id);if(e is null)return NotFound();if(e.ParticipantID!=UserId)return Forbid();if(e.Status=="Cancelled")return NoContent();e.Status="Cancelled";await db.SaveChangesAsync();return NoContent();}
}
