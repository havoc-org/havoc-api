using Havoc_API.Data;
using Havoc_API.Services;
using Microsoft.EntityFrameworkCore;
using WarehouseApp2.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IParticipationService, ParticipationService>();
builder.Services.AddScoped<IHavocContext, HavocContext>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HavocContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
