using Booking.App.Services;
using Booking.MAUI.ViewModels;

namespace Booking.MAUI.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel homeViewModel)
        {
            InitializeComponent();
            BindingContext = homeViewModel;  
        }
    }
}
