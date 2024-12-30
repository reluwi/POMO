using Microsoft.Maui.Controls;

namespace POMO
{
    public partial class StartingPage : ContentPage
    {
        public StartingPage()
        {
            InitializeComponent();
        }

        private async void OnStartButtonClicked(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Shell.Current.GoToAsync("MainPage");
        }
    }
}