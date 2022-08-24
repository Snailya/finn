using FINN.API;
using FINN.CORE.Interfaces;
using FINN.CORE.Models;
using FINN.PLUGINS.EFCORE;
using FINN.PLUGINS.EFCORE.Data;
using FINN.SHAREDKERNEL;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IBroker, RabbitMqBroker>();
builder.Services.AddDbContext(builder.Configuration.GetConnectionString("SqliteConnection"));
builder.Services.AddScoped<IRepository<Job>, EfRepository<Job>>();
builder.Services.AddHostedService<HostedService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.WithOrigins("https://localhost").AllowAnyOrigin().WithExposedHeaders("location"); });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enforce lowercase routing
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


// Ensure database created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();