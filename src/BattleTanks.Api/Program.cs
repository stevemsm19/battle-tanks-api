using BattleTanks.Api.Middlewares;
using BattleTanks.Api.Extensions;
using BattleTanks.Application.Interfaces;
using BattleTanks.Application.Services.Auth;
using BattleTanks.Application.Services.Users;
using BattleTanks.Domain.Interfaces;
using BattleTanks.Domain.Interfaces.Users;
using BattleTanks.Infrastructure.Persistence;
using BattleTanks.Infrastructure.Persistence.Repositories;
using BattleTanks.Infrastructure.Persistence.Repositories.Users;
using BattleTanks.Infrastructure.Persistence.Repositories.Rooms;
using BattleTanks.Domain.Interfaces.Rooms;
using BattleTanks.Application.Interfaces.Rooms;
using BattleTanks.Application.Services.Rooms;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using BattleTanks.Infrastructure.Hubs;
using BattleTanks.Application.Common.Interfaces;
using BattleTanks.Infrastructure.Services.SignalR;

var builder = WebApplication.CreateBuilder(args);

var firebaseCredentialPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-service-account.json");
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseCredentialPath)
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BattleTanksDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });
});

builder.Services.Configure<RouteOptions>(options =>
    options.LowercaseUrls = true);
builder.Services.AddScoped<IIdentityProvider, FirebaseIdentityRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddFirebaseAuthentication(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddScoped<IGameNotificationService, GameNotificationService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("AllowFrontendApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserTokenValidationMiddleware>();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();
