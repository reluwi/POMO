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

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            // Navigate to the SignUpPage or perform any action
            await Shell.Current.GoToAsync("//SignUpPage");
        }
    }
}