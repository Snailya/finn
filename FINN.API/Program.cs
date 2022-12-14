using FINN.API.Data;
using FINN.API.Models;
using FINN.BROKER.RABBITMQ;
using FINN.CORE.Interfaces;
using FINN.REPOSITORY.EFCORE;
using FINN.SHAREDKERNEL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IBroker, RabbitMqBroker>();
builder.Services.AddDbContextFactory<AppDbContext>(builder.Configuration.GetConnectionString("SqliteConnection"));
builder.Services.AddScoped<IRepositoryFactory<RequestLog>, RepositoryFactory<RequestLog, AppDbContext>>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.SetIsOriginAllowed(origin =>
            {
                var uri = new Uri(origin);
                return uri.Host is "10.25.141.134" or "localhost";
            }).AllowAnyHeader().AllowAnyMethod();
            // cors is controlled by AllowedHosts in appsettings.json
            // policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
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