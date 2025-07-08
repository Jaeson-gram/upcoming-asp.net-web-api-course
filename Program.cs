using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WebAPI2.Configurations;
using WebAPI2.Data;
using WebAPI2.Data.Repository;
using WebAPI2.Logs;

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetValue<string>("jwtSecret") ??
          "this is the most ultimate secret: every secret will be revealed";


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

// cors
// builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.WithOrigins("http://localhost:3000")));


builder.Services.AddCors(options =>
{

    options.AddDefaultPolicy(policy =>
    {
        //allow all origins
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });

    // origins can be added per policy and customised as desired...
    options.AddPolicy("corsCustomOriginAndHeaders", policy => 
    { 
            //allow specific origins/headers/methods/headers
            policy.WithOrigins("http://localhost:5120", "http://localhost:5140", "https://www.yourapp.com").WithMethods("GET", "POST");
    });
    
    options.AddPolicy("LocalHost", policyBuilder => 
    { 
            //allow only localhost test origins/headers/methods/headers
            policyBuilder.WithOrigins("http://localhost:5120", "http://localhost:5140").AllowAnyMethod().AllowAnyHeader();
    });

    options.AddPolicy(name: "OnlyGroupTest", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://www.yourgroupproject.com", "https://www.yourmailingservice.com", "https://www.yourcallservice.com").WithMethods("GET", "POST");
    });
    
    options.AddPolicy(name: "ForHiring", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://www.geniuses.com", "https://www.skillplace.com", "https://www.workaholic.com").WithMethods("GET", "POST");
    });

});
   
// jwt auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(); // uses default
// app.UseCors("OnlyGroupTest"); // specific 

// app.MapControllers();
app.UseAuthorization();


/*
 * this overrides middleware and EnableCors attribute,
 * requires the app.UseRouting() called, and app.UseCors() placed between it app.UseEndpoints()
 * and requires app.MapControllers() inside the endpoint body.

 * Also, it is unnecessary and recommendation is that the EnableCors should be used in addition to Middleware, instead of endpoint routing at all...

app.UseEndpoints(endpoints =>
    {
        // without specifying a policy, it uses the default - which makes the 'RequireCors' unnecessary anyway..
        endpoints.MapControllers().RequireCors(); 
        
        //uses the specified cors policy for the specified endpoint. overrides middleware and EnableCors attribute
       endpoints.MapGet("api/endpoint1", 
           context => context.Response.WriteAsync("Hello World!")).RequireCors("LocalHost"); 
       
       // without using the RequireCors and specifying which cors policy to use, it applies the default policy to ths endpoint specified
       endpoints.MapGet("api/endpoint2", 
           context => context.Response.WriteAsync("Hello World!"));  
    });

*/

app.MapControllers(); 
app.Run();

