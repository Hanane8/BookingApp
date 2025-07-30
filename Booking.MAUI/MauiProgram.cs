using Microsoft.Extensions.Logging;
using Booking.App.Mappings;
using Booking.App.Services;
using Booking.App.DTOs;
using Booking.App.Validators;
using FluentValidation;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using Booking.MAUI.Views;
using Booking.Database.Repositories;
using Booking.MAUI.ViewModels;
using Booking.Database.Database;
using Microsoft.EntityFrameworkCore;
using Booking.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Booking.MAUI.Service;

namespace Booking.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Lägg till AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddSingleton<AuthService>();

            builder.Services.AddDbContext<BokningContext>(options =>
            {
                options.UseSqlServer("Server=Hanane\\SQLEXPRESS01; Database=BookingApp; Trusted_Connection=True; TrustServerCertificate=True;");
            });

            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IBookingService, BookingService>();
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IValidator<BookingDto>, BookingValidator>();

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            builder.Services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();

            // Register pages
            builder.Services.AddScoped<HomePage>();
            builder.Services.AddScoped<LoginPage>();
            builder.Services.AddScoped<HomeViewModel>();
            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<MyBookingsPage>();
            builder.Services.AddScoped<MyBookingsViewModel>();
           

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
