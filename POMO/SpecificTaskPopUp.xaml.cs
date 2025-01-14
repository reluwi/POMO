using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Graphics;
using CommunityToolkit.Maui.Views;

namespace POMO
{
	public partial class SpecificTaskPopUp : Popup
	{
        public required string DueDate { get; set; }
        public required string TaskTitle { get; set; }
        public required string Description { get; set; }
        public required string NumSession { get; set; }

        public event Action<string, string, DateTime, int>? EditRequested;

        private TaskModel? _task;
        public SpecificTaskPopUp()
		{
			InitializeComponent();
        }

        public void SetTask(TaskModel task)
        {
            _task = task;

            // Set the task information
            DueDateLabel.Text = $"Due Date: {task.DueDate:MM/dd/yyyy}";
            TaskTitleLabel.Text = task.Title ?? "No title";
            DescriptionLabel.Text = task.Description ?? "No description";
            NumSessionLabel.Text = $"Number of Sessions: {task.NumSessions}";

            // Update the NotStarted label based on the number of CompletedSessions
            if (task.CompletedSessions >= 2)
            {
                NotStarted.Text = "Started";
            }
            else
            {
                NotStarted.Text = "Not started";
            }

            // Debug statement to verify task initialization
            Console.WriteLine($"SpecificTaskPopUp initialized with task: {task.Title}");
        }

        private async void OnGoTimerButtonClicked(object sender, EventArgs e)
        {
            if (_task == null)
            {
                Console.WriteLine("Task is null.");
                return;
            }

            // Debug statement to verify task information
            Console.WriteLine($"Navigating to TimerPage with task: {_task.Title}");

            // Send the task information to the TimerPage
            WeakReferenceMessenger.Default.Send(new TaskSelectedMessage(_task));

            // Navigate to the TimerPage
            await Shell.Current.GoToAsync("TimerPage");

            // Close the popup
            this.Close();
        }

        public void DisplayTaskDetails(string dueDate, string taskTitle, string description, string numSession)
        {
            DueDateLabel.Text = dueDate; 
            TaskTitleLabel.Text = taskTitle; 
            DescriptionLabel.Text = description;
            NumSessionLabel.Text = numSession;

            // Make the main popup content visible
            var mainPopupContent = this.FindByName<Border>("MainPopupContent");
            if (mainPopupContent != null)
            {
                mainPopupContent.IsVisible = true;
            }
            this.Show();
        }

        public void Show()
        {
            // Show the popup using the current page
            var currentPage = Application.Current?.Windows[0].Page;
            if (currentPage != null)
            {
                currentPage.ShowPopup(this);
            }
        }

        public void Hide()
        {
            this.Close();
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Hide();
        }

        private void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Extract values from the labels
            string taskTitle = TaskTitleLabel.Text;
            string taskDescription = DescriptionLabel.Text;

            // Parse the due date (assuming the format is valid)
            DateTime dueDate;
            if (!DateTime.TryParse(DueDateLabel.Text.Replace("DUE on ", ""), out dueDate))
            {
                // If parsing fails, set the due date to the current date
                dueDate = DateTime.Now;
            }

            // Parse the number of sessions (assuming it's a valid number)
            int.TryParse(NumSessionLabel.Text.Replace("Number of Session : ", ""), out int numSessions);

            // Trigger the EditRequested event and pass the extracted values
            EditRequested?.Invoke(taskTitle, taskDescription, dueDate, numSessions);

            // Show the EditTaskPopUp
            var editTaskPopUp = new EditTaskPopUp
            {
                TaskTitleEntryControl = { Text = taskTitle },
                DescriptionEditorControl = { Text = taskDescription },
                DatePickerControl = { Date = dueDate },
                SessionCountLabelControl = { Text = numSessions > 0 ? numSessions.ToString() : "1" }
            };

            // Show the EditTaskPopUp using the current page
            var currentPage = Application.Current?.Windows[0].Page;
            if (currentPage != null)
            {
                currentPage.ShowPopup(editTaskPopUp);
            }

            // Optionally, hide SpecificTaskPopUp
            this.Close();
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            // Show the DeleteTaskPopUp
            var deleteTaskPopUp = new DeleteTaskPopUp();

            // Show the DeleteTaskPopUp using the current page
            var currentPage = Application.Current?.Windows[0].Page;
            if (currentPage != null)
            {
                currentPage.ShowPopup(deleteTaskPopUp);
            }

            // Optionally, hide SpecificTaskPopUp
            this.Close();
        }

        private void OnMarkAsDoneButtonClicked(object sender, EventArgs e)
        {
            // Traverse the visual tree to find TaskPage
            Element parent = this;

            while (parent != null)
            {
                if (parent is TaskPage taskPage)
                {
                    if (taskPage.selectedTaskBorder != null)
                    {
                        // Call the MarkAsDone function in TaskPage
                        taskPage.OnMarkAsDoneClicked(sender, e);

                        // Hide the popup
                        this.Close();
                        return;
                    }
                }

                parent = parent.Parent;
            }

            Console.WriteLine("TaskPage not found in the visual tree.");
        }

        public void HideButtons()
        {
            MarkAsDoneButton.IsVisible = false;
            GoTimerButton.IsVisible = false;
            EditButton.IsEnabled = false;
            EditButton.Opacity = 0;
            DueDateLabel.TextColor = Color.FromArgb("#30BFBF");
            GoTimerLogo.IsVisible = false;
            NotStarted.Text = "Finished";
            DescriptionLabel.HorizontalOptions = LayoutOptions.Center;
            DescriptionLabel.HorizontalTextAlignment = TextAlignment.Center;
            NumSessionLabel.HorizontalOptions = LayoutOptions.Center;
        }

        public void ShowButtons()
        {
            MarkAsDoneButton.IsVisible = true;
            GoTimerButton.IsVisible = true;
            EditButton.IsVisible = true;
            DueDateLabel.TextColor = Color.FromArgb("#FF6F61");
        }

    }
}