using Microsoft.EntityFrameworkCore;
using Oceanarium.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Oceanarium.Servises.Interfaces;
using Oceanarium.Servises;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Oceanarium.Middleware;
using Oceanarium.Identity;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddRazorPages();

//Identity
builder.Services.AddIdentity<AdminUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Create roles for the first time
async Task CreateRolesAsync(IServiceProvider serviceProvider)
{

    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

//Login
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied"; 
});

//Custom services and other
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//Singleton DI
builder.Services.AddSingleton<IAdminKeyService, AdminKeyService>();

//Scope
builder.Services.AddHostedService<UsersCleanUpService>();
builder.Services.AddHostedService<CheckDateService>();

//Transient DI
builder.Services.AddTransient<IDiscountService, DiscountService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IQrCodeCreator, QrCodeCreator>();
builder.Services.AddTransient<IFilterTicketService, FilterTicketService>();
builder.Services.AddTransient<IFilterOrderService, FilterOrderService>();
builder.Services.AddTransient<IFilterEventService, FilterEventService>();
builder.Services.AddTransient<IFilterExibitionService, FilterExibitionService>();





var app = builder.Build();

//Initialize roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRolesAsync(services);
}

//For numbers and dates
var cultureInfo = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<AdminAreaMiddleware>();

app.MapRazorPages();

app.Run();
