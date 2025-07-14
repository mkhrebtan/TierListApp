using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TierList.Application;
using TierList.Application.Common.Settings;
using TierList.Infrastructure;
using TierList.Persistence.Postgres;
using TierListAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var assemblyName = typeof(Program).Assembly.GetName().Name;

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey)),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

app.Services.EnsureDataInitialized();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Tier List API";
        options.HideClientButton = true;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapTierListEndpoints();

app.MapTierRowEndpoints();

app.MapTierImageEndpoints();

await app.RunAsync();