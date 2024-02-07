using Malefashion;
using Malefashion.DAL;
using Malefashion.Interfaces;
using Malefashion.Models;
using Malefashion.Services;
using Malefashion.ViewComponents;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbConfig(builder.Configuration);
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
	opt.Password.RequireNonAlphanumeric=false;

	opt.User.RequireUniqueEmail = true;

    
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<HeaderViewComponent>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(

	"default",
	"{area:exists}/{controller=Home}/{action=Index}/{id?}"

	);
app.MapControllerRoute(
	
	"default",
	"{controller=Home}/{action=Index}/{id?}"
	
	);

app.Run();
