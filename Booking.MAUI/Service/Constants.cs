using Microsoft.Maui.Devices;

namespace Booking.MAUI.Service
{
    public static class Constants
    {
        public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7006" // Android emulator's "localhost" with HTTPS
            : "https://localhost:7006"; // Development machine's "localhost" with HTTPS
    }
} 