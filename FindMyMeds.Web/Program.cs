using FindMyMeds.Core.Entities;
using FindMyMeds.Core.Identity;
using FindMyMeds.Core.Interfaces;
using FindMyMeds.Infrastructure.Data;
using FindMyMeds.Infrastructure.Repositories;
using FindMyMeds.Infrastructure.Seed;
using FindMyMeds.Services.Background;
using FindMyMeds.Services.Interfaces;
using FindMyMeds.Services.Managers;
using FindMyMeds.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()

    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)

    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "FindMyMeds")

    .WriteTo.File(
        path: "Logs/general-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e =>
            e.Properties.ContainsKey("Category") &&
            e.Properties["Category"].ToString().Contains("Inventory"))
        .WriteTo.File("Logs/inventory-.txt", rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e =>
            e.Properties.ContainsKey("Category") &&
            e.Properties["Category"].ToString().Contains("Pharmacy"))
        .WriteTo.File("Logs/pharmacy-.txt", rollingInterval: RollingInterval.Day))

    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
        .WriteTo.File("Logs/errors-.txt", rollingInterval: RollingInterval.Day))

    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepository<Pharmacy>, PharmacyRepository>();

builder.Services.AddScoped<IMedicationManager, MedicationManager>();
builder.Services.AddScoped<IPharmacyManager, PharmacyManager>();
builder.Services.AddScoped<IPharmacyMedicationManager, PharmacyMedicationManager>();
builder.Services.AddScoped<IReportManager, ReportManager>();    

builder.Services.AddSignalR();

builder.Services.AddScoped<IMedicationNotifier, SignalRMedicationNotifier>();

builder.Services.AddHostedService<LowStockNotificationService>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "FindMyMeds API",
        Version = "v1",
        Description = "Public API for medication availability and pharmacy inventory"
    });
});



var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
            
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FindMyMeds API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapHub<FindMyMeds.Web.Hubs.MedicationHub>("/medicationHub");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await RoleSeeder.SeedAsync(services);

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string adminEmail = "admin@findmymeds.com";
    string adminPassword = "Admin@123";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    await UserSeeder.SeedAsync(services);
}


app.Run();
