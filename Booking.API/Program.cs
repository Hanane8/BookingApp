using Booking.App.Mappings;
using Booking.App.Services;
using Booking.Database.Database;
using Booking.Database.DataSeedHelper;
using Booking.Database.Entities;
using Booking.Database.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using Booking.App.Validators;
using Booking.App.DTOs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IValidator<BookingDto>, BookingValidator>();
builder.Services.AddTransient<IValidator<LoginDto>,  LoginDtoValidator>();
builder.Services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();

//builder.Services.AddScoped<BookingValidator>();
//builder.Services.AddScoped<UserValidator>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});




builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHttpClient<IUserService, UserService>();
//builder.Services.AddHttpClient<IUserService, UserService>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]); 
//});





builder.Services.AddScoped<IBookingService, BookingService>();

// Register DbContext
builder.Services.AddDbContext<BookingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register SeedHelper
builder.Services.AddTransient<SeedHelper>();
// Register PasswordHasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking API v1");
        c.RoutePrefix = string.Empty; 
    });
}

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedHelper.Seed(services);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();