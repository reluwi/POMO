using System;
using System.Timers;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace POMO
{
	public partial class TimerPage : ContentPage
	{
        private bool isTimerRunning = false;
        private TimeSpan timeRemaining = TimeSpan.FromMinutes(25);
        private System.Timers.Timer timer;
        public TimerPage()
		{
			InitializeComponent();

            // Initialize the timer
            timer = new System.Timers.Timer(1000); // 1-second interval
            timer.Elapsed += OnTimerElapsed!;
        }

        private void OnPlayPauseButtonClicked(object? sender, EventArgs e)
        {
            if (sender is not ImageButton playPauseButton)
                return; // Exit early if sender is not an ImageButton

            if (isTimerRunning)
            {
                // Pause the timer
                isTimerRunning = false;
                timer.Stop();
                playPauseButton.Source = "play_button.png";
            }
            else
            {
                // Start the timer
                isTimerRunning = true;
                timer.Start();
                playPauseButton.Source = "pause_button.png";
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Update the remaining time
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));

            // Update the TimerLabel text on the main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = timeRemaining.ToString(@"mm\:ss");

                if (timeRemaining <= TimeSpan.Zero)
                {
                    // Stop the timer when time is up
                    timer.Stop();
                    isTimerRunning = false;

                    // Reset the play button
                    var playPauseButton = this.FindByName<ImageButton>("PlayPauseButton");
                    playPauseButton.Source = "play_button.png";

                    // Optional: Add logic to notify the user (e.g., vibration, sound)
                }
            });
        }

        private void EndTimer_Clicked(object sender, EventArgs e)
        {
            this.ShowPopup(new EndTimerPopUp());
        }

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("MainPage");
        }

        private async void OnTaskButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TaskPage
            await Shell.Current.GoToAsync("TaskPage");
        }
    }
}