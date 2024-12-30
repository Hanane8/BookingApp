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

            builder.Services.AddHttpClient("ApiHttpClient", httpClient =>
            {
                // Replace localhost with your machine's IP address
                var baseAddress = DeviceInfo.Platform == DevicePlatform.Android
                    ? "http://10.0.2.2:5133"  // For Android Emulator
                    : "http://192.168.0.103:5133"; // For other platforms (like iOS, Windows)

                httpClient.BaseAddress = new Uri(baseAddress);
            });

            // Add other services
            builder.Services.AddDbContext<BookingContext>(options =>
            {
                options.UseSqlServer("Server=Hanane\\SQLEXPRESS01; Database=BookingApp; Trusted_Connection=True; TrustServerCertificate=True;");
            });

            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IBookingService, BookingService>();
            builder.Services.AddSingleton<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            builder.Services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();

            // Register pages
            builder.Services.AddScoped<HomePage>();
            builder.Services.AddScoped<LoginPage>();
            builder.Services.AddScoped<HomeViewModel>();
            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
