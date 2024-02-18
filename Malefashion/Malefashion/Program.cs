using Malefashion;
using Malefashion.DAL;
using Malefashion.Interfaces;
using Malefashion.Middlewares;
using Malefashion.Models;
using Malefashion.Services;
using Malefashion.ViewComponents;
using Microsoft.AspNetCore.Identity;

using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddViewComponentsAsServices(); ;
builder.Services.AddDbConfig(builder.Configuration);
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
	opt.Password.RequireNonAlphanumeric=false;

	opt.User.RequireUniqueEmail = true;

    opt.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<HeaderViewComponent>();
builder.Services.AddScoped<FooterViewComponent>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailService, MailService>();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
StripeConfiguration.ApiKey = builder.Configuration["Stripe:Secretkey"];
app.MapControllerRoute(

	"default",
	"{area:exists}/{controller=Home}/{action=Index}/{id?}"

	);
app.MapControllerRoute(
	
	"default",
	"{controller=Home}/{action=Index}/{id?}"
	
	);

app.Run();
