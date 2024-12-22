namespace POMO
{

	public partial class RunningTimePage : ContentPage
	{
		public RunningTimePage()
		{
			InitializeComponent();
		}

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Shell.Current.GoToAsync("///MainPage");
        }

        private async void OnClockButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("TimerPage");
        }
    }
}