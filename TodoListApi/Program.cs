using dotenv.net;
using Microsoft.EntityFrameworkCore;
using TodoListApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotEnv.Load();

// Retrieve PostgreSQL configuration from environment variables
var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
var username = Environment.GetEnvironmentVariable("POSTGRES_USER");
var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

// Add services to the container.
builder.Services.AddControllers();


// Register the DbContext and configure it to use PostgreSQL
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();