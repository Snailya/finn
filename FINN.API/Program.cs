using FINN.API.Data;
using FINN.API.Models;
using FINN.CORE.Interfaces;
using FINN.PLUGINS.EFCORE;
using FINN.SHAREDKERNEL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IBroker, RabbitMqBroker>();
builder.Services.AddDbContext<AppDbContext>(builder.Configuration.GetConnectionString("SqliteConnection"));
builder.Services.AddScoped<DbContext, AppDbContext>();
builder.Services.AddScoped<IRepository<RequestLog>, EfRepository<RequestLog>>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // only allow request from localhost
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enforce lowercase routing
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Create database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();