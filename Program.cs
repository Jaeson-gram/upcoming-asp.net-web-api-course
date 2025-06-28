using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using WebAPI2.Configurations;
using WebAPI2.Data;
using WebAPI2.Data.Repository;
using WebAPI2.Logs;

var builder = WebApplication.CreateBuilder(args);
// AppContext.SetSwitch("System.Globalization.Invariant", false);

// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// builder.Logging.AddDebug();

// Serilog
Log.Logger = new LoggerConfiguration().WriteTo.File(path: "serilog/logs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

// => override default loggers and use only serilog instead of using the ClearProviders() above
// builder.Host.UseSerilog();

// => use serilog in addition to default loggers: Debug and Console
builder.Logging.AddSerilog();           // builder.Host.ConfigureLogging(logging => logging.AddSerilog(dispose: true));




// Add services to the container.

builder.Services.AddDbContext<SchoolDbContext>(options =>
{
    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=SchoolDB;Integrated Security=True;Trust Server Certificate=True");

    // options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")); using the name in app.dev.json wasn't working
});

builder.Services.AddControllers(
    // options => options.ReturnHttpNotAcceptable = true
).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// automapper 
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//dependency injection --> whenever we use the ILog in the ctor, use the {second argument below} for it. ()
builder.Services.AddTransient<ILog, LogToDb>();
builder.Services.AddTransient<IStudentRepository, StudentRepository>();
builder.Services.AddTransient(typeof(ISchoolRepository<>), typeof(SchoolRepository<>));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

