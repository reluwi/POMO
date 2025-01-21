using System;
using System.Timers;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Maui.Controls.Shapes;
using System.Linq;
using Plugin.Maui.Audio; // Add this for sound notification

namespace POMO
{
    public partial class TimerPage : ContentPage
    {
        private bool isTimerRunning = false;
        private TimeSpan timeRemaining = TimeSpan.FromMinutes(25);  //adjust timer
        //private TimeSpan timeRemaining = TimeSpan.FromSeconds(10);
        private System.Timers.Timer timer;
        private IAudioManager audioManager;

        private const string TaskIdKey = "TaskId";
        private const string TaskTitleKey = null;
        private const string CompletedSessionsKey = "CompletedSessions";
        private const string SkipSessionButtonVisibleKey = "SkipSessionButtonVisible";
        private const string ResetButtonVisibleKey = "ResetButtonVisible";
        private const string DefaultTButtonVisibleKey = "DefaultTButtonVisible";
        private const string EndTaskButtonVisibleKey = "EndTaskButtonVisible";
        private const string EndTimerButtonVisibleKey = "EndTimerButtonVisible";

        public TimerPage()
        {
            InitializeComponent();

            // Initialize the timer
            timer = new System.Timers.Timer(1000); // 1-second interval
            timer.Elapsed += OnTimerElapsed!;

            // Initialize the audio manager
            audioManager = AudioManager.Current;

            // Register to receive the TaskSelectedMessage
            WeakReferenceMessenger.Default.Register<TaskSelectedMessage>(this, (r, m) =>
            {
                SetTask(m.Value.Id, $"{m.Value.Title} ({m.Value.CompletedSessions} / {m.Value.NumSessions})", m.Value.CompletedSessions);
                var getTaskTitle = $"{m.Value.Title}";
                Preferences.Set("TaskTitle", getTaskTitle);
            });

            // Register to receive the TaskSelectedMessage
            WeakReferenceMessenger.Default.Register<DueTaskSelectedMessage>(this, (r, m) =>
            {
                SetTask(m.Value.Id, $"{m.Value.Title} ({m.Value.CompletedSessions} / {m.Value.NumSessions})", m.Value.CompletedSessions);
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Debug statement to verify OnAppearing is called
            Console.WriteLine("OnAppearing called");

            // Retrieve the selected task information from Preferences
            var taskTitle = Preferences.Get(TaskTitleKey, string.Empty);
            var taskId = Preferences.Get(TaskIdKey, -1);
            var completedSessions = Preferences.Get(CompletedSessionsKey, 1);

            // Debug statements to verify preferences are retrieved correctly
            Console.WriteLine($"TaskTitle: {taskTitle}");
            Console.WriteLine($"TaskId: {taskId}");
            Console.WriteLine($"CompletedSessions: {completedSessions}");

            if (taskId != -1 && !string.IsNullOrEmpty(taskTitle))
            {
                TaskTitle.Text = taskTitle;
                ChooseButton.Text = "+ Choose Another Task";
            }
            else
            {
                // Set the default task if no task is currently selected
                SetDefaultTask();
            }

            // Retrieve button visibility states from Preferences
            SkipSessionButton.IsVisible = Preferences.Get(SkipSessionButtonVisibleKey, false);
            ResetButton.IsVisible = Preferences.Get(ResetButtonVisibleKey, false);
            DefaultTButton.IsVisible = Preferences.Get(DefaultTButtonVisibleKey, false);
            EndTaskButton.IsVisible = Preferences.Get(EndTaskButtonVisibleKey, false);
            EndTimerButton.IsVisible = Preferences.Get(EndTimerButtonVisibleKey, true);
        }

        private void SetDefaultTask()
        {
            // Set the TaskTitle to default
            TaskTitle.Text = "Pomodoro Timer";

            // Clear the selected task information from Preferences
            Preferences.Remove(TaskTitleKey);
            Preferences.Remove(TaskIdKey);
            Preferences.Remove(CompletedSessionsKey);

            // Set a flag indicating that the default task is set
            Preferences.Set("IsDefaultTaskSet", true);

            // Update UI
            SkipSessionButton.IsVisible = false;
            ResetButton.IsVisible = false;
            DefaultTButton.IsVisible = false;
            EndTaskButton.IsVisible = false;
            EndTimerButton.IsVisible = false;

            ChooseButton.Text = "+ Choose a Task";

            // Store button visibility states in Preferences
            Preferences.Set(SkipSessionButtonVisibleKey, false);
            Preferences.Set(ResetButtonVisibleKey, false);
            Preferences.Set(DefaultTButtonVisibleKey, false);
            Preferences.Set(EndTaskButtonVisibleKey, false);
            Preferences.Set(EndTimerButtonVisibleKey, false);
        }

        public void SetTask(int taskId, string taskTitle, int completedSessions)
        {
            Preferences.Set("IsDefaultTaskSet", false);

            // Update the TimerLabel with the selected task information
            TaskTitle.Text = taskTitle;

            SkipSessionButton.IsVisible = true;
            ResetButton.IsVisible = true;
            DefaultTButton.IsVisible = true;
            EndTaskButton.IsVisible = true;
            EndTimerButton.IsVisible = false;

            ChooseButton.Text = "+ Choose Another Task";

            // Store the selected task information in Preferences
            Preferences.Set(TaskIdKey, taskId);
            Preferences.Set(TaskTitleKey, taskTitle);
            Preferences.Set(CompletedSessionsKey, completedSessions);

            // Store button visibility states in Preferences
            Preferences.Set(SkipSessionButtonVisibleKey, true);
            Preferences.Set(ResetButtonVisibleKey, true);
            Preferences.Set(DefaultTButtonVisibleKey, true);
            Preferences.Set(EndTaskButtonVisibleKey, true);
            Preferences.Set(EndTimerButtonVisibleKey, false);
        }

        private async void ChooseTask_Clicked(object sender, EventArgs e)
        {
            var chooseTaskPopUp = new ChooseTaskPopUp();
            chooseTaskPopUp.LoadTasksFromTaskPage();
            var result = await this.ShowPopupAsync(chooseTaskPopUp);

            if (result is Tuple<int, string, int> taskResult)
            {
                var taskId = taskResult.Item1;
                var taskTitle = taskResult.Item2;
                var completedSessions = taskResult.Item3;

                // Update the TimerLabel with the selected task information
                TaskTitle.Text = taskTitle;

                SkipSessionButton.IsVisible = true;
                ResetButton.IsVisible = true;
                DefaultTButton.IsVisible = true;
                EndTaskButton.IsVisible = true;
                EndTimerButton.IsVisible = false;

                ChooseButton.Text = "+ Choose Another Task";

                // Store the selected task information in Preferences
                Preferences.Set(TaskIdKey, taskId);
                Preferences.Set(TaskTitleKey, taskTitle);
                Preferences.Set(CompletedSessionsKey, completedSessions);

                // Store button visibility states in Preferences
                Preferences.Set(SkipSessionButtonVisibleKey, true);
                Preferences.Set(ResetButtonVisibleKey, true);
                Preferences.Set(DefaultTButtonVisibleKey, true);
                Preferences.Set(EndTaskButtonVisibleKey, true);
                Preferences.Set(EndTimerButtonVisibleKey, false);
            }
        }

        //posible polymorphism application here
        private async void EndTimerButton_Clicked(object sender, EventArgs e)
        {
            // Show the popup
            var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to end the timer?"));

            // Check if the result is "Continue"
            if (result is string action && action == "Continue")
            {
                // Play sound notification
                var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("notification_sound.mp3"));
                player.Play();

                // Reset the timer to 25:00 and pause
                timeRemaining = TimeSpan.FromMinutes(25);
                //timeRemaining = TimeSpan.FromSeconds(10);
                isTimerRunning = false;
                timer.Stop();
                ChooseButton.IsVisible = true;
                EndTimerButton.IsVisible = false;

                // Update UI
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerLabel.Text = "25:00";
                    //TimerLabel.Text = "00:10";
                    PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown

                    // Clear the selected task information from Preferences
                    Preferences.Remove(TaskIdKey);
                    Preferences.Remove(TaskTitleKey);
                    Preferences.Remove(CompletedSessionsKey);

                    // Store button visibility states in Preferences
                    Preferences.Set(SkipSessionButtonVisibleKey, false);
                    Preferences.Set(ResetButtonVisibleKey, false);
                    Preferences.Set(DefaultTButtonVisibleKey, false);
                    Preferences.Set(EndTaskButtonVisibleKey, false);
                    Preferences.Set(EndTimerButtonVisibleKey, false);
                });
            }
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
                ChooseButton.IsVisible = false;
                DefaultTButton.IsVisible = false;
                EndTimerButton.IsVisible = true;
                var taskId = Preferences.Get(TaskIdKey, -1);
                if (taskId != -1)
                {
                    EndTimerButton.IsVisible = false;
                }
            }
        }

        private async void SkipSession_Clicked(object sender, EventArgs e)
        {
            // Show the popup with the custom text for skipping a session
            var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to skip this session?"));

            // Check if the result is "Continue"
            if (result is string action && action == "Continue")
            {
                // Play sound notification
                var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("notification_sound.mp3"));
                player.Play();

                // Increment the session counter for the selected task
                var taskId = Preferences.Get(TaskIdKey, -1);
                if (taskId != -1)
                {
                    var task = await App.Database.GetTaskAsync(taskId);
                    if (task != null)
                    {
                        task.CompletedSessions++;
                        await App.Database.SaveTaskAsync(task);

                        // Check if the task is completed
                        if (task.CompletedSessions > task.NumSessions)
                        {
                            task.IsCompleted = true;
                            await App.Database.SaveTaskAsync(task);

                            // Move the task to the completed tasks in TaskPage
                            var taskPage = Application.Current?.Windows[0]?.Page?.Navigation?.NavigationStack?.OfType<TaskPage>().FirstOrDefault();
                            if (taskPage != null)
                            {
                                taskPage.AddCompletedTask(new Border
                                {
                                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                                    BackgroundColor = Colors.White,
                                    Padding = new Thickness(15),
                                    BindingContext = task
                                });
                            }

                            // Clear the selected task information from Preferences
                            Preferences.Remove(TaskIdKey);
                            Preferences.Remove(TaskTitleKey);
                            Preferences.Remove(CompletedSessionsKey);

                            // Set a flag indicating that the default task is set
                            Preferences.Set("IsDefaultTaskSet", true);

                            // Reset the timer to 25:00 and pause
                            timeRemaining = TimeSpan.FromMinutes(25);
                            //timeRemaining = TimeSpan.FromSeconds(10);
                            isTimerRunning = false;
                            timer.Stop();
                            ChooseButton.IsVisible = true;
                            DefaultTButton.IsVisible = true;

                            // Update UI
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                TimerLabel.Text = "25:00";
                                //TimerLabel.Text = "00:10";
                                PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                                SetDefaultTask();
                            });

                            await DisplayAlert("Task Completed", "The task has been moved to completed tasks.", "OK");
                        }
                        else
                        {
                            // Update the task information
                            var taskTitle = $"{task.Title} ({task.CompletedSessions} / {task.NumSessions})";
                            TaskTitle.Text = taskTitle;

                            // Store the updated task information in Preferences
                            Preferences.Set(TaskTitleKey, taskTitle);
                            Preferences.Set(CompletedSessionsKey, task.CompletedSessions);

                            // Reset the timer to 25:00 and pause
                            timeRemaining = TimeSpan.FromMinutes(25);
                            //timeRemaining = TimeSpan.FromSeconds(10);
                            isTimerRunning = false;
                            timer.Stop();
                            ChooseButton.IsVisible = true;
                            DefaultTButton.IsVisible = true;

                            // Update UI
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                TimerLabel.Text = "25:00";
                                //TimerLabel.Text = "00:10";
                                PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                            });
                        }
                    }
                }
                await DisplayAlert("Session Completed", "You have completed a session. Please take a 5-minute break.", "OK");
            }
        }

        private async void OnResetButtonClicked(object sender, EventArgs e)
        {
            // Show the popup with the custom text for resetting the timer
            var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to reset the timer?"));

            // Check if the result is "Continue"
            if (result is string action && action == "Continue")
            {
                // Reset the timer to 25:00 and pause
                timeRemaining = TimeSpan.FromMinutes(25);
                //timeRemaining = TimeSpan.FromSeconds(10);
                isTimerRunning = false;
                timer.Stop();
                ChooseButton.IsVisible = true;
                DefaultTButton.IsVisible = true;

                // Update UI
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerLabel.Text = "25:00";
                    //TimerLabel.Text = "00:10";
                    PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                });
            }
        }

        private async void EndTaskButton_Clicked(object sender, EventArgs e)
        {
            // Show the popup with the custom text for ending the task
            var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to end this task?"));

            // Check if the result is "Continue"
            if (result is string action && action == "Continue")
            {
                // Play sound notification
                var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("notification_sound.mp3"));
                player.Play();

                // Mark the selected task as completed
                var taskId = Preferences.Get(TaskIdKey, -1);
                if (taskId != -1)
                {
                    var task = await App.Database.GetTaskAsync(taskId);
                    if (task != null)
                    {
                        task.IsCompleted = true;
                        await App.Database.SaveTaskAsync(task);

                        // Move the task to the completed tasks in TaskPage
                        var taskPage = Application.Current?.Windows[0]?.Page?.Navigation?.NavigationStack?.OfType<TaskPage>().FirstOrDefault();
                        if (taskPage != null)
                        {
                            taskPage.AddCompletedTask(new Border
                            {
                                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                                BackgroundColor = Colors.White,
                                Padding = new Thickness(15),
                                BindingContext = task
                            });
                        }

                        // Clear the selected task information from Preferences
                        Preferences.Remove(TaskIdKey);
                        Preferences.Remove(TaskTitleKey);
                        Preferences.Remove(CompletedSessionsKey);

                        // Set a flag indicating that the default task is set
                        Preferences.Set("IsDefaultTaskSet", true);

                        // Reset the timer to 25:00 and pause
                        timeRemaining = TimeSpan.FromMinutes(25);
                        //timeRemaining = TimeSpan.FromSeconds(10);
                        isTimerRunning = false;
                        timer.Stop();
                        ChooseButton.IsVisible = true;
                        DefaultTButton.IsVisible = true;

                        // Update UI
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            TimerLabel.Text = "25:00";
                            //TimerLabel.Text = "00:10";
                            PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                            SetDefaultTask();
                        });

                        await DisplayAlert("Task Completed", "The task has been moved to completed tasks. Please take a 20/30-minute break.", "OK");
                    }
                }
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // Update the remaining time
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));

            // Update the TimerLabel text on the main thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                TimerLabel.Text = timeRemaining.ToString(@"mm\:ss");

                if (timeRemaining <= TimeSpan.Zero)
                {
                    // Stop the timer when time is up
                    timer.Stop();
                    isTimerRunning = false;

                    // Play sound notification
                    var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("notification_sound.mp3"));
                    player.Play();

                    // Display alert for session completion
                    await DisplayAlert("Session Completed", "You have completed a session. Please take a 5-minute break.", "OK");

                    // Reset the timer to 25:00 and pause
                    timeRemaining = TimeSpan.FromMinutes(25);
                    //timeRemaining = TimeSpan.FromSeconds(10);
                    isTimerRunning = false;
                    timer.Stop();
                    ChooseButton.IsVisible = true;
                    EndTimerButton.IsVisible = false;
                    DefaultTButton.IsVisible = false;

                    // Update UI
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TimerLabel.Text = "25:00";
                        //TimerLabel.Text = "00:10";
                        PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                    });

                    // Increment the session counter for the selected task
                    var taskId = Preferences.Get(TaskIdKey, -1);
                    if (taskId != -1)
                    {
                        var task = await App.Database.GetTaskAsync(taskId);
                        if (task != null)
                        {
                            task.CompletedSessions++;
                            await App.Database.SaveTaskAsync(task);

                            // Check if the task is completed
                            if (task.CompletedSessions > task.NumSessions)
                            {
                                task.IsCompleted = true;
                                await App.Database.SaveTaskAsync(task);

                                // Move the task to the completed tasks in TaskPage
                                var taskPage = Application.Current?.Windows[0]?.Page?.Navigation?.NavigationStack?.OfType<TaskPage>().FirstOrDefault();
                                if (taskPage != null)
                                {
                                    taskPage.AddCompletedTask(new Border
                                    {
                                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                                        BackgroundColor = Colors.White,
                                        Padding = new Thickness(15),
                                        BindingContext = task
                                    });
                                }

                                // Clear the selected task information from Preferences
                                Preferences.Remove(TaskIdKey);
                                Preferences.Remove(TaskTitleKey);
                                Preferences.Remove(CompletedSessionsKey);

                                // Reset the timer to 25:00 and pause
                                timeRemaining = TimeSpan.FromMinutes(25);
                                //timeRemaining = TimeSpan.FromSeconds(10);
                                isTimerRunning = false;
                                timer.Stop();
                                ChooseButton.IsVisible = true;
                                DefaultTButton.IsVisible = true;

                                // Set a flag indicating that the default task is set
                                Preferences.Set("IsDefaultTaskSet", true);

                                // Update UI
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    TimerLabel.Text = "25:00";
                                    //TimerLabel.Text = "00:10";
                                    PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                                    SetDefaultTask();
                                });

                                await DisplayAlert("Task Completed", "The task has been moved to completed tasks. Please take a 20/30-minute break.", "OK");
                            }
                            else
                            {
                                // Update the task information
                                var taskTitle = $"{task.Title} ({task.CompletedSessions} / {task.NumSessions})";
                                TaskTitle.Text = taskTitle;

                                // Store the updated task information in Preferences
                                Preferences.Set(TaskTitleKey, taskTitle);
                                Preferences.Set(CompletedSessionsKey, task.CompletedSessions);

                                // Reset the timer to 25:00 and pause
                                timeRemaining = TimeSpan.FromMinutes(25);
                                //timeRemaining = TimeSpan.FromSeconds(10);
                                isTimerRunning = false;
                                timer.Stop();
                                ChooseButton.IsVisible = true;
                                DefaultTButton.IsVisible = true;

                                // Update UI
                                MainThread.BeginInvokeOnMainThread(() =>
                                {
                                    TimerLabel.Text = "25:00";
                                    //TimerLabel.Text = "00:10";
                                    PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown
                                });
                            }
                        }
                    }
                }
            });
        }

        private void DefaultButton_Clicked(object sender, EventArgs e)
        {
            SetDefaultTask();
        }

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            if (TimerLabel.Text != "25:00")
            {
                // Show the popup
                var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to end the timer?"));

                // Check if the result is "Continue"
                if (result is string action && action == "Continue")
                {
                    // Reset the timer to 25:00 and pause
                    timeRemaining = TimeSpan.FromMinutes(25);
                    isTimerRunning = false;
                    timer.Stop();
                    ChooseButton.IsVisible = true;

                    // Update UI
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TimerLabel.Text = "25:00";
                        PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown

                        // Clear the selected task information from Preferences
                        Preferences.Remove(TaskIdKey);
                        Preferences.Remove(TaskTitleKey);
                        Preferences.Remove(CompletedSessionsKey);

                        // Store button visibility states in Preferences
                        Preferences.Set(SkipSessionButtonVisibleKey, false);
                        Preferences.Set(ResetButtonVisibleKey, false);
                        Preferences.Set(DefaultTButtonVisibleKey, false);
                        Preferences.Set(EndTaskButtonVisibleKey, false);
                        Preferences.Set(EndTimerButtonVisibleKey, true);
                    });

                    // Navigate to HomePage
                    await Shell.Current.GoToAsync("MainPage");
                }
            }
            else
            {
                // Navigate to HomePage
                await Shell.Current.GoToAsync("MainPage");
            }
        }

        private async void OnTaskButtonTapped(object sender, EventArgs e)
        {
            if (TimerLabel.Text != "25:00")
            {
                // Show the popup
                var result = await this.ShowPopupAsync(new EndTimerPopUp("Are you sure you want to end the timer?"));

                // Check if the result is "Continue"
                if (result is string action && action == "Continue")
                {
                    // Reset the timer to 25:00 and pause
                    timeRemaining = TimeSpan.FromMinutes(25);
                    isTimerRunning = false;
                    timer.Stop();
                    ChooseButton.IsVisible = true;

                    // Update UI
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TimerLabel.Text = "25:00";
                        PlayPauseButton.Source = "play_button.png"; // Ensure play button is shown

                        // Clear the selected task information from Preferences
                        Preferences.Remove(TaskIdKey);
                        Preferences.Remove(TaskTitleKey);
                        Preferences.Remove(CompletedSessionsKey);

                        // Store button visibility states in Preferences
                        Preferences.Set(SkipSessionButtonVisibleKey, false);
                        Preferences.Set(ResetButtonVisibleKey, false);
                        Preferences.Set(DefaultTButtonVisibleKey, false);
                        Preferences.Set(EndTaskButtonVisibleKey, false);
                        Preferences.Set(EndTimerButtonVisibleKey, true);
                    });

                    // Navigate to TaskPage
                    await Shell.Current.GoToAsync("TaskPage");
                }
            }
            else
            {
                // Navigate to TaskPage
                await Shell.Current.GoToAsync("TaskPage");
            }
        }
    }
}