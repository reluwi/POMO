namespace POMO
{
    [QueryProperty(nameof(InitialTimeValue), "InitialTimeValue")]
    public partial class RunningTimePage : ContentPage
	{
        private double remainingTime;

        // Property to receive the time value
        public double InitialTimeValue { get; set; }
        public RunningTimePage()
		{
			InitializeComponent();

            // Listen for the SkipConfirmed event from the SkipPopUp
            SkipPopup.SkipConfirmed += OnSkipConfirmed;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Ensure InitialTimeValue is applied
            remainingTime = Math.Floor(InitialTimeValue);
            RunningTimerLabel.Text = $"{(int)remainingTime}:00";

            StartTimerWithDelay();
        }

        private async void StartTimerWithDelay()
        {
            // Add a 2-second delay before the timer begins
            await Task.Delay(300);

            // Start the timer after the delay
            StartTimer();
        }

        private void StartTimer()
        {
            // Use the Dispatcher to start the timer
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (remainingTime > 0)
                {
                    remainingTime -= 1.0 / 60; // Decrement by 1 second
                    UpdateTimerLabel();
                    return true; // Continue the timer
                }
                else
                {
                    // Timer has completed
                    Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Time's Up!", "The session has ended.", "OK");
                    });
                    return false; // Stop the timer
                }
            });
        }

        private void UpdateTimerLabel()
        {
            int minutes = (int)Math.Floor(remainingTime);
            int seconds = (int)((remainingTime - minutes) * 60);
            
            // Ensure seconds always start at 0
            if (seconds < 0) seconds = 0;

            RunningTimerLabel.Text = $"{minutes:D2}:{seconds:D2}"; // Update the label
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