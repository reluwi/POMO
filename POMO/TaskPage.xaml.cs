namespace POMO
{
    public partial class TaskPage : ContentPage
    {
        private bool isExistingTasksVisible = true;
        private bool isCompletedTasksVisible = true;
        public TaskPage()
        {
            InitializeComponent();

            // Access the TaskPopUp instance from XAML and subscribe to the events
            TaskPopUp.TaskCreated += OnTaskCreated;
            TaskPopUp.Cancelled += OnTaskCancelled;

        }

        private void OnExistingTasksToggleClicked(object sender, EventArgs e)
        {
            isExistingTasksVisible = !isExistingTasksVisible;
            ExistingTasksContent.IsVisible = isExistingTasksVisible;

            // Change the arrow icon
            if (isExistingTasksVisible)
            {
                ExistingTasksToggle.Source = "arrow_up.png";
            }
            else
            {
                ExistingTasksToggle.Source = "arrow_down.png";
            }
        }

        private void OnCompletedTasksToggleClicked(object sender, EventArgs e)
        {
            isCompletedTasksVisible = !isCompletedTasksVisible;
            CompletedTasksContent.IsVisible = isCompletedTasksVisible;

            // Change the arrow icon
            if (isCompletedTasksVisible)
            {
                CompletedTasksToggle.Source = "arrow_up.png";
            }
            else
            {
                CompletedTasksToggle.Source = "arrow_down.png";
            }
        }

        private void OnCreateNewTaskClicked(object sender, EventArgs e)
        {
            TaskPopUp.IsVisible = true; // Show the TaskPopUp when the button is clicked
        }

        // Event handler when a task is created (done button clicked)
        private void OnTaskCreated(object? sender, EventArgs e)
        {
            DisplayAlert("Task Created", "A new task has been successfully created.", "OK");
            TaskPopUp.IsVisible = false; // Hide the popup after task is created
        }

        // Event handler when task creation is canceled (cancel button clicked)
        private void OnTaskCancelled(object? sender, EventArgs e)
        {
            TaskPopUp.IsVisible = false; // Hide the popup when the action is canceled
        }

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("MainPage");
        }
    }
}