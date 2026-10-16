using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.DTOs;
using RaceDay.API.Services;
namespace RaceDay.API.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase {
    /// <summary>Creates a Participant account, or an Organiser account when a valid organiser code is supplied.</summary>
    [HttpPost("register"),AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse),201)] [ProducesResponseType(400)]
    public async Task<IActionResult> Register(RegisterRequest request) {
        if(!ModelState.IsValid) return ValidationProblem(ModelState);
        var (user,error)=await auth.Register(request); if(user is null) return BadRequest(new MessageResponse(error!));
        return Created("/api/users/me",new UserResponse(user.UserID,user.FirstName,user.LastName,user.Email,user.Role,user.CreatedAt));
    }
    /// <summary>Validates credentials and establishes an HTTP-only authentication cookie.</summary>
    [HttpPost("login"),AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request) {
        var user=await auth.Login(request.Email,request.Password); if(user is null) return Unauthorized(new MessageResponse("Email or password is incorrect."));
        var claims=new List<Claim>{new(ClaimTypes.NameIdentifier,user.UserID.ToString()),new(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),new(ClaimTypes.Email,user.Email),new(ClaimTypes.Role,user.Role)};
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));
        return Ok(new UserResponse(user.UserID,user.FirstName,user.LastName,user.Email,user.Role,user.CreatedAt));
    }
    /// <summary>Ends the current authenticated session.</summary>
    [HttpPost("logout"),Authorize]
    public async Task<IActionResult> Logout(){await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);return Ok(new MessageResponse("Logged out."));}
}
