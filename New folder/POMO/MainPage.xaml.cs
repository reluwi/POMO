namespace POMO
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnClockButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("TimerPage");
        }

        private async void OnNotifButtonTapped(object sender, EventArgs e)
        {
            // Navigate to NotifPage
            await Shell.Current.GoToAsync("NotifPage");
        }
    }

}
