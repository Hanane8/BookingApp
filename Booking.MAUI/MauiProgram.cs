using Microsoft.Extensions.Logging;
using Booking.App.DTOs;
using Booking.App.Validators;
using FluentValidation;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;
using Booking.MAUI.Views;
using Booking.MAUI.ViewModels;
using Booking.MAUI.Service;
using System.Net.Http;

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

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7006/")
            };

            // Registrera HttpClient som Singleton (en enda instans)
            builder.Services.AddSingleton<HttpClient>(httpClient);

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ConcertService>();
            builder.Services.AddSingleton<PerformanceService>();
            builder.Services.AddSingleton<BookingHttpService>();
            builder.Services.AddScoped<IValidator<BookingDto>, BookingValidator>();

            builder.Services.AddTransient<IValidator<RegisterDto>, RegisterDtoValidator>();
            builder.Services.AddTransient<IValidator<LoginDto>, LoginDtoValidator>();

            // Register pages
            builder.Services.AddScoped<HomePage>();
            builder.Services.AddScoped<LoginPage>();
            builder.Services.AddScoped<HomeViewModel>();
            builder.Services.AddScoped<LoginViewModel>();
            builder.Services.AddScoped<MyBookingsPage>();
            builder.Services.AddScoped<MyBookingsViewModel>();
            builder.Services.AddScoped<UpdateBookingViewModel>();
           

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
