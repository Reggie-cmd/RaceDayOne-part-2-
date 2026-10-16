using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
namespace RaceDay.API.Services;
public class EnrolmentService(RaceDayDbContext db) {
    public async Task<(Enrolment? enrolment,string? error)> Create(int participantId,int categoryId) {
        var category=await db.Categories.Include(c=>c.Event).FirstOrDefaultAsync(c=>c.CategoryID==categoryId);
        if(category is null) return (null,"Category not found.");
        if(category.Event.Status!="Open" || category.Event.EventDate.Date < DateTime.UtcNow.Date) return (null,"This event is not open for enrolment.");
        if(category.MaxParticipants is int max && await db.Enrolments.CountAsync(e=>e.CategoryID==categoryId && e.Status!="Cancelled")>=max) return (null,"This category is full.");
        if(await db.Enrolments.AnyAsync(e=>e.ParticipantID==participantId && e.CategoryID==categoryId)) return (null,"You are already enrolled in this category.");
        var item=new Enrolment { ParticipantID=participantId,CategoryID=categoryId,Status="Confirmed",EnrolmentDate=DateTime.UtcNow };
        db.Enrolments.Add(item); await db.SaveChangesAsync(); return (item,null);
    }
}
