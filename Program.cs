using Microsoft.EntityFrameworkCore;
using purpuraMain.DbContext;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

// PostgreSQL Database connection using dependency injection

builder.Services.AddDbContextPool<PurpuraDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
