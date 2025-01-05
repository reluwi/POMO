using Microsoft.Maui.Graphics;

namespace POMO
{
	public partial class SpecificTaskPopUp : ContentView
	{
        public required string DueDate { get; set; }
        public required string TaskTitle { get; set; }
        public required string Description { get; set; }
        public required string NumSession { get; set; }

        public event Action<string, string, DateTime, int>? EditRequested;

        public SpecificTaskPopUp()
		{
			InitializeComponent();
        }

        public void DisplayTaskDetails(string dueDate, string taskTitle, string description, string numSession)
        {
            DueDateLabel.Text = dueDate; // Example: XAML element with x:Name="DueDateLabel"
            TaskTitleLabel.Text = taskTitle; // Example: XAML element with x:Name="TaskTitleLabel"
            DescriptionLabel.Text = description;
            NumSessionLabel.Text = numSession;
            //IsVisible = true;
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        public void Hide()
        {
            this.IsVisible = false;
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
            DateTime.TryParse(DueDateLabel.Text, out DateTime dueDate);

            // Parse the number of sessions (assuming it's a valid number)
            int.TryParse(NumSessionLabel.Text.Replace("Number of Session : ", ""), out int numSessions);

            // Trigger the EditRequested event and pass the extracted values
            EditRequested?.Invoke(taskTitle, taskDescription, dueDate, numSessions);

            // Traverse the visual tree to find the TaskPage
            Element parent = this;

            while (parent != null)
            {
                if (parent is TaskPage taskPage)
                {
                    // Show EditTaskPopUp
                    taskPage.EditTaskPopUpInstance.IsVisible = true;

                    // Optionally, hide SpecificTaskPopUp
                    this.IsVisible = false;

                    return;
                }

                // Traverse to the next parent
                parent = parent.Parent;
            }

            // If TaskPage was not found, log a message (optional)
            Console.WriteLine("TaskPage not found in the visual tree.");
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            // Traverse the visual tree to find the TaskPage
            Element parent = this;

            while (parent != null)
            {
                if (parent is TaskPage taskPage)
                {
                    // Show the DeleteTaskPopUp
                    taskPage.DeleteTaskPopUpInstance.IsVisible = true;

                    // Optionally, hide the SpecificTaskPopUp
                    this.IsVisible = false;

                    return;
                }

                // Traverse to the next parent
                parent = parent.Parent;
            }

            // If TaskPage was not found, log a message (optional)
            Console.WriteLine("TaskPage not found in the visual tree.");
        }

    }
}