using FINN.API;
using FINN.API.Contexts;
using FINN.CORE;
using FINN.SHAREDKERNEL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IBroker, RabbitMqBroker>();
builder.Services.AddDbContextFactory<JobContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("JobContextSQLite")));
builder.Services.AddHostedService<HostedService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.WithOrigins("https://localhost").AllowAnyOrigin().WithExposedHeaders("location"); });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enforce lowercase routing
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


// Create database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<JobContext>();
    context.Database.EnsureCreated();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();