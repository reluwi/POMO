using CommunityToolkit.Maui.Views;

namespace POMO
{
	public partial class TimerPage : ContentPage
	{
        //private int _pomodoroCount = 1;
        public TimerPage()
		{
			InitializeComponent();
		}

        private void EndTimer_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new TimerPopUpPage1());
        }

        private void ChooseTask_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new TimerPopUpPage2());
        }

        public void ShowPopup()
        {
            this.ShowPopup(new TimerPopUpPage3());
        }

        private void Pomodoro_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new TimerPopUpPage3());
        }

        private void SkipSession_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new TimerPopUpPage4());
        }

        /*
        private void OnIncreasePomodoroClicked(object sender, EventArgs e)
        {
            // Increment the Pomodoro count
            _pomodoroCount++;

            // Update the label
            PomodoroCountLabel.Text = _pomodoroCount.ToString();
        }

        private void OnDecreasePomodoroClicked(object sender, EventArgs e)
        {
            // Decrement the Pomodoro count, ensuring it stays >= 1
            if (_pomodoroCount > 1)
            {
                _pomodoroCount--;
            }

            // Update the label
            PomodoroCountLabel.Text = _pomodoroCount.ToString();
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

        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            // Get the value directly from the slider
            var timeSet = TimeSlider.Value;

            // Navigate to RunningTimePage with the selected time
            await Shell.Current.GoToAsync(
                $"{nameof(RunningTimePage)}?InitialTimeValue={timeSet}&PomodoroCount={_pomodoroCount}");
        }
        */
    }
}