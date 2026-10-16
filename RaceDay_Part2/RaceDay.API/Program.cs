using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RaceDay.API.Data;
using RaceDay.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RaceDay API", Version = "v1", Description = "Race event management API for organisers and participants." });
    options.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme { Name = ".RaceDay.Auth", Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Cookie, Description = "Log in through POST /api/auth/login first, then use the authentication cookie." });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Cookie" } }, Array.Empty<string>() } });
    var xml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(xml)) options.IncludeXmlComments(xml);
});
builder.Services.AddDbContext<RaceDayDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RaceDay")));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<EnrolmentService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.Cookie.Name = ".RaceDay.Auth"; options.Cookie.HttpOnly = true; options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict; options.SlidingExpiration = true; options.ExpireTimeSpan = TimeSpan.FromHours(4);
    options.Events.OnRedirectToLogin = context => { context.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
    options.Events.OnRedirectToAccessDenied = context => { context.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("OrganiserOnly", p => p.RequireAuthenticatedUser().RequireRole("Organiser"));
    options.AddPolicy("ParticipantOnly", p => p.RequireAuthenticatedUser().RequireRole("Participant"));
});
builder.Services.AddCors(options => options.AddPolicy("LocalDevelopment", p => p.WithOrigins("https://localhost:5173", "http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseCors("LocalDevelopment");
app.UseAuthentication(); app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", application = "RaceDay API" })).WithTags("Health").WithSummary("Check API health.");
app.Run();
public partial class Program { }
