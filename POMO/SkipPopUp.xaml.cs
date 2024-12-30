namespace POMO
{
	public partial class SkipPopUp : ContentView
	{
        // Declare the event as nullable
        public event EventHandler? SkipConfirmed;

        public SkipPopUp()
		{
            InitializeComponent();  
		}

        private void OnCancelTapped(object sender, EventArgs e)
        {
            this.IsVisible = false; // Hide the pop-up
        }

        private async void OnSkipConfirmedTapped(object sender, EventArgs e)
        {
            SkipConfirmed?.Invoke(this, EventArgs.Empty); // Notify the parent page
            this.IsVisible = false; // Hide the pop-up
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("///TimerPage");
        }
    }
}