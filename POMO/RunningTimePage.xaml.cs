namespace POMO
{

	public partial class RunningTimePage : ContentPage
	{
		public RunningTimePage()
		{
			InitializeComponent();

            // Listen for the SkipConfirmed event from the SkipPopUp
            SkipPopup.SkipConfirmed += OnSkipConfirmed;
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

        private void OnSkipButtonTapped(object sender, EventArgs e)
        {
            SkipPopup.IsVisible = true; // Show the pop-up
        }

        private void OnSkipConfirmed(object? sender, EventArgs e)
        {
            // Logic when the skip is confirmed
            DisplayAlert("Skipped", "You skipped the current focus session.", "OK");
        }
    }
}