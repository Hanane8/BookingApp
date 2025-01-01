using Booking.MAUI.ViewModels;

namespace Booking.MAUI.Views;

public partial class MyBookingsPage : ContentPage
{
	public MyBookingsPage( MyBookingsViewModel MyBookingsViewModel)
	{
		InitializeComponent();
        BindingContext = MyBookingsViewModel;
    }
}