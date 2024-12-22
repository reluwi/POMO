namespace POMO
{
	public partial class TimerPage : ContentPage
	{
		public TimerPage()
		{
			InitializeComponent();
		}

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Get the slider value
            var sliderValue = e.NewValue;

            // Update the timer label (formatted as mm:ss)
            TimerLabel.Text = $"{(int)sliderValue}:00";
        }

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Shell.Current.GoToAsync("///MainPage");
        }
    }
}