using Microsoft.Maui.Controls;

namespace POMO
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            // Navigate to the MainPage
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}