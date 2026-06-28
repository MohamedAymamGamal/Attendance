using Hangfire;
using Hangfire.MemoryStorage;
using Job.Data;
using Job.Jobs;
using Job.Mapping;
using Job.Reposities;
using Job.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddControllers();



//server layer
/////////////////////////////////////////////////
builder.Services.AddScoped<AttendanceRepo>();
builder.Services.AddScoped<AttendanceExcelService>();
//////////////////////////////////////////////////


//////////////////////HANGFIRE
builder.Services.AddHangfire(c => c.UseMemoryStorage());
builder.Services.AddHangfireServer();
//////////////////////

///////////////////////////////////////////////////////
builder.Services.AddScoped<AttendanceExcelService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//////////////////////////////////////////////////////

builder.Services.AddOpenApi();

var app = builder.Build();

/////////////////
RecurringJob.AddOrUpdate<AttendanceJob>(
    "end-of-day-attendance",
    job => job.Execute(),
    "59 23 * * *"   
);
/////////////////

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
