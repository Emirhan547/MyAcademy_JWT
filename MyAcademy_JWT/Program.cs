using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyAcademy_JWT.Data.Context;
using MyAcademy_JWT.Data.Repositories;
using MyAcademy_JWT.Data.Repositories.PackageRepositories;
using MyAcademy_JWT.Data.Repositories.SongRepositories;
using MyAcademy_JWT.Data.Repositories.UserRepositories;
using MyAcademy_JWT.Data.Repositories.UserSongHistoryRepositories;
using MyAcademy_JWT.Endpoints;
using MyAcademy_JWT.Entities;
using MyAcademy_JWT.Models.MLNetViewModels;
using MyAcademy_JWT.Options;
using MyAcademy_JWT.Services;
using MyAcademy_JWT.Services.AuthServices;
using MyAcademy_JWT.Services.HistoryServices;
using MyAcademy_JWT.Services.PackageAccessServices;     // ✅ TEK DOĞRU NAMESPACE
using MyAcademy_JWT.Services.PlayServices;
using MyAcademy_JWT.Services.RecommendationServices;
using MyAcademy_JWT.Services.SongQueryServices;
using MyAcademy_JWT.Services.TokenServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ISongRepository, SongRepository>();
builder.Services.AddScoped<IUserSongHistoryRepository, UserSongHistoryRepository>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IPackageAccessService, PackageAccessService>(); // ✅
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ISongQueryService, SongQueryService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

// ML
builder.Services.AddSingleton<MLModelState>();
builder.Services.AddHostedService<RecommendationBackgroundService>();

// Auth (JWT Cookie)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
    };

    // ✅ JWT'yi cookie'den oku
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            ctx.Token = ctx.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");

// Minimal API endpoints
app.MapAuthEndpoints();
app.MapPingEndpoints();
app.MapSongEndpoints();
app.MapPlayerEndpoints();
app.MapHistoryEndpoints();
app.MapRecommendationEndpoints();

app.Run();