using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using WebAPIProject.Container;
using WebAPIProject.Helper;
using WebAPIProject.Model;
using WebAPIProject.Repos;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add dependency injection
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddDbContext<LearndataContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("apiconn")));

//add basic authenctication
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

//add jwt atuhentication
var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

//congfigure automapper
var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

//add CORS
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
}));
// * means our api can be accesed for any app or domain

builder.Services.AddCors(p => p.AddPolicy("corspolicy1", build =>
{
    build.WithOrigins("https://domain3.com").AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.AddCors(p => p.AddDefaultPolicy( build =>
{
    build.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
}));


//add rate limiting
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    //so after exceuting the request after 1 limit it will give error and in next 10 sec it will give data
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);

//logging
string logpath = builder.Configuration.GetSection("Logging:Logpath").Value;
var _logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(logpath)
    .CreateLogger();
builder.Logging.AddSerilog(_logger);


//jwtsetting
var _jwtsetting = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(_jwtsetting);

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//MINIMAL api
app.MapGet("/minimalapi", () => "chirag Mali");

app.MapGet("/getchannel", (string channelname) => "Welcome to "+channelname);
//.WithOpenApi(opt =>
//{
//    var parameter = opt.Parameters[0];
//    parameter.Description = "Enter Channel Name";
//    return opt;
//});

app.MapGet("/getcustomer", async (LearndataContext db) =>
{
    return await db.Customers.ToListAsync();
});

app.MapGet("/getcustomerbycode/{code}", async (LearndataContext db, int code) =>
{
    return await db.Customers.FindAsync(code);
});

app.MapPost("/createcustomer", async (LearndataContext db, Customer customer) =>
{
     await db.Customers.AddAsync(customer);
    await db.SaveChangesAsync();
});

app.MapPut("/updatecustomer/{code}", async (LearndataContext db, Customer customer,int code) =>
{
    var existdata = await db.Customers.FindAsync(code);
    if(existdata != null)
    {
        existdata.Name = customer.Name;
        existdata.Email = customer.Email;
    }
    await db.SaveChangesAsync();
});

app.MapDelete("/removecustomer/{code}", async (LearndataContext db, int code) =>
{
    var existdata = await db.Customers.FindAsync(code);
    if (existdata != null)
    {
        db.Customers.Remove(existdata);
    }
    await db.SaveChangesAsync();
});

//enabling CORS globally using middlewar else  we can do wiht controller level also but its rarely
app.UseCors();

//enable rate limiter
app.UseRateLimiter();

//enable static files to access the host url
app.UseStaticFiles();

app.UseHttpsRedirection();

//enable authentication for both jwt and basics
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
