namespace POMO
{
    public partial class TaskPage : ContentPage
    {
        private bool isExistingTasksVisible = true;
        private bool isCompletedTasksVisible = true;

        public TaskPage()
        {
            InitializeComponent();

            TaskPopUp.Initialize(this);

            // Access the TaskPopUp instance from XAML and subscribe to the events
            TaskPopUp.TaskCreated += OnTaskCreated;
            TaskPopUp.Cancelled += OnTaskCancelled;
        }

        // Update the method in TaskPopUp.xaml.cs to reference ExistingTasksContent directly
        public void AddNewTask(Border taskBorder)
        {
            // Ensure ExistingTasksContent is directly accessible
            ExistingTasksContent.Children.Add(taskBorder);
        }

        public new async Task DisplayAlert(string title, string message, string cancel)
        {
            // Use the inherited DisplayAlert from ContentPage by explicitly hiding it here
            await base.DisplayAlert(title, message, cancel);
        }

        private void OnExistingTasksToggleClicked(object sender, EventArgs e)
        {
            /*isExistingTasksVisible = !isExistingTasksVisible;
            ExistingTasksContent.IsVisible = isExistingTasksVisible;

            // Change the arrow icon
            if (isExistingTasksVisible)
            {
                ExistingTasksToggle.Source = "arrow_up.png";
            }
            else
            {
                ExistingTasksToggle.Source = "arrow_down.png";
            }*/
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

        public void OnTaskTapped(object sender, EventArgs e)
        {
            // Get the tapped Border
            var border = sender as Border;
            if (border == null)
                return;

            // Extract task details (labels inside the Border)
            var stackLayout = border.Content as VerticalStackLayout;
            if (stackLayout == null)
                return;

            var dueDateLabel = stackLayout.Children[0] as Label;
            var taskTitleLabel = stackLayout.Children[1] as Label;
            var descriptionLabel = stackLayout.Children[2] as Label;
            var NumSessionLabel = stackLayout.Children[3] as Label;

            if (dueDateLabel != null && taskTitleLabel != null && descriptionLabel != null && NumSessionLabel != null)
            {
                // Pass the task details to the SpecificTaskPopUp
                SpecificTaskPopUp.DisplayTaskDetails(dueDateLabel.Text, taskTitleLabel.Text, descriptionLabel.Text, NumSessionLabel.Text);

                // Show the pop-up
                //SpecificTaskPopUp.IsVisible = true;
            }
        }

        // Event handler when a task is created (done button clicked)
        private void OnTaskCreated(object? sender, EventArgs e)
        {
            _ = DisplayAlert("Task Created", "A new task has been successfully created.", "OK");
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