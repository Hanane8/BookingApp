using Microsoft.Maui.Devices;

namespace Booking.MAUI.Service
{
    public static class Constants
    {
        public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5133" // Android emulator's "localhost"
            : "http://localhost:5133"; // Development machine's "localhost"
    }
} 