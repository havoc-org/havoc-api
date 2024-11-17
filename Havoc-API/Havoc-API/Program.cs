using Havoc_API.Data;
using Havoc_API.Services;
using Havoc_API.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Text;
using WarehouseApp2.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)),
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ValidAudience = builder.Configuration["JWT:Audience"],
           ClockSkew = TimeSpan.FromMinutes(1)
       };
       options.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               var authHeader = context.Request.Headers.Authorization.ToString();
               if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
               {
                   context.Token = authHeader["Bearer ".Length..].Trim();
                   context.HttpContext.Items["JwtToken"] = context.Token;
               }
               return Task.CompletedTask;
           }
       };
   });
Console.WriteLine(builder.Configuration["JWT:Issuer"]);
Console.WriteLine(builder.Configuration["JWT:Audience"]);
Console.WriteLine(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!)));
Console.WriteLine(builder.Configuration["JWT:Audience"]);
// Add services to the container.
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IParticipationService, ParticipationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHavocContext, HavocContext>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();


builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddDbContext<HavocContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
               .WithOrigins("https://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/error"
});

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
