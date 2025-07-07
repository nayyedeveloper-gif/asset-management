using AMS.ConHelper;
using AMS.Data;
using AMS.Helpers;
using AMS.Models;
using AMS.Models.CommonViewModel;
using AMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Security headers
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});

builder.Services.AddScoped<ApplicationDbContext>();
var _ApplicationInfo = builder.Configuration.GetSection("ApplicationInfo").Get<ApplicationInfo>();
string _GetConnStringName = ControllerExtensions.GetConnectionString(builder.Configuration);

if (_ApplicationInfo.DBConnectionStringName == ConnectionStrings.connMySQL)
{
    builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(_GetConnStringName, ServerVersion.AutoDetect(_GetConnStringName)));
}
else if(_ApplicationInfo.DBConnectionStringName == ConnectionStrings.connPostgreSQL)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_GetConnStringName));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_GetConnStringName));
}

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 3;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Identity cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);

// Identity options, DefaultIdentityOptions, SuperAdminDefaultOptions, ApplicationInfo configuration
builder.Services.Configure<SuperAdminDefaultOptions>(builder.Configuration.GetSection("SuperAdminDefaultOptions"));
builder.Services.Configure<ApplicationInfo>(builder.Configuration.GetSection("ApplicationInfo"));
builder.Services.Configure<DefaultIdentityOptions>(builder.Configuration.GetSection("IdentityDefaultOptions"));

builder.Services.AddTransient<ICommon, Common>();
builder.Services.AddTransient<IAccount, Account>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IRoles, Roles>();
builder.Services.AddTransient<IFunctional, Functional>();
builder.Services.AddTransient<INotificationService, NotificationService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

// Swagger configuration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Asset Management System", Version = "v1" });
        c.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
            In = ParameterLocation.Header,
            Name = "Authorization",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        });
    });
}

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", builder => 
    {
        builder.WithOrigins("https://localhost:5001", "http://localhost:5000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
    
    options.AddPolicy("Development", builder => 
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMS v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseSession();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Database initialization - moved to after app build
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Note: Identity options are already configured in the service registration above
        // Database-specific identity options can be implemented later if needed
        
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var functional = services.GetRequiredService<IFunctional>();

        DbInitializer.Initialize(context, functional).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
