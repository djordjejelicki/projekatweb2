using AutoMapper;
using BLL.Services.Implementations;
using BLL.Services.Interfaces;
using BLL.Services.Mapping;
using DAL.Context;
using DAL.Repository;
using DAL.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IItemService, ItemService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddSingleton<IShipmentService, ShipmentService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle("Google", options => 
{
    options.ClientId = "834446938128-d491ro7ugt5tmc9ia3brtlq7i89e61tq.apps.googleusercontent.com";
    options.ClientSecret = "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";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters //Podesavamo parametre za validaciju pristiglih tokena
    {
        ValidateIssuer = true, //Validira izdavaoca tokena
        ValidateAudience = false, //Kazemo da ne validira primaoce tokena
        ValidateLifetime = true,//Validira trajanje tokena
        ValidateIssuerSigningKey = true, //validira potpis token, ovo je jako vazno!
        ValidIssuer = "http://localhost:5000", //odredjujemo koji server je validni izdavalac
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]))//navodimo privatni kljuc kojim su potpisani nasi tokeni
    };
});
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
