using Malefashion;
using Malefashion.DAL;
using Malefashion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbConfig(builder.Configuration);
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
	opt.Password.RequireNonAlphanumeric=false;

	opt.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


var app = builder.Build();

app.UseRouting();


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
