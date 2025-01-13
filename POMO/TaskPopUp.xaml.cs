using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.VisualBasic;

namespace POMO
{
    public partial class TaskPopUp : Popup
    {
        // Define events for Task creation and cancellation
        public event EventHandler? TaskCreated;

        private TaskPage? _taskPage;

        public TaskPopUp()
        {
            InitializeComponent();

            // Set the DatePicker to the current date
            DatePickerControl.Date = DateTime.Now;
        }

        public void Initialize(TaskPage taskPage)
        {
            _taskPage = taskPage;
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            // Parse the current session count
            int currentCount = int.Parse(SessionCountLabelControl.Text);

            // Increment the session count
            currentCount++;

            // Update the label
            SessionCountLabelControl.Text = currentCount.ToString();
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            // Parse the current session count
            int currentCount = int.Parse(SessionCountLabelControl.Text);

            // Decrement the session count only if it's greater than 1
            if (currentCount > 1)
            {
                currentCount--;
            }

            // Update the label
            SessionCountLabelControl.Text = currentCount.ToString();
        }

        private void OnDescriptionTextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the length of the user's input
            int currentLength = e.NewTextValue.Length;

            // Update the character count label
            CharacterCountLabel.Text = $"Description {currentLength}/100";
        }


        // Event handler when the Cancel button is clicked
        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close();
        }

        // Method to hide the pop-up
        public void Hide()
        {
            TaskTitleEntryControl.Text = string.Empty;
            DescriptionEditorControl.Text = string.Empty;
            SessionCountLabelControl.Text = "1";

            // Set the DatePicker to the current date
            DatePickerControl.Date = DateTime.Now;


            this.Close();
        }

        public void Show()
        {
            TaskTitleEntryControl.Text = string.Empty;
            DescriptionEditorControl.Text = string.Empty;
            SessionCountLabelControl.Text = "1";

            // Set the DatePicker to the current date
            DatePickerControl.Date = DateTime.Now;

            this.Show();
        }

        // Method to show alert using the parent page
        private void ShowAlert(string title, string message)
        {
            _taskPage?.DisplayAlert(title, message, "OK");
        }

        // Done button click handler
        private async void OnDoneClicked(object sender, EventArgs e)
        {
            // Get the values from the user inputs
            string taskTitle = TaskTitleEntryControl.Text?.Trim() ?? string.Empty;
            string taskDescription = DescriptionEditorControl.Text?.Trim() ?? string.Empty;

            // Ensure all necessary fields are filled
            if (string.IsNullOrEmpty(taskTitle) || string.IsNullOrEmpty(taskDescription))
            {
                // Handle error: Show a message that fields are missing
                ShowAlert("Error", "Please fill out all the fields before proceeding.");
                return;
            }

            // Construct the due date from the selected values
            DateTime dueDate = DatePickerControl.Date;

            // Get the session count
            int sessionCount = int.TryParse(SessionCountLabelControl.Text, out var result) ? result : 1;

            var newTask = new TaskModel
            {
                Title = taskTitle,
                Description = taskDescription,
                DueDate = dueDate,
                NumSessions = sessionCount,
                IsCompleted = false
            };

            // Save the task to the database
            await App.Database.SaveTaskAsync(newTask);

            // Send a message to notify that a new task has been added
            WeakReferenceMessenger.Default.Send(new TaskAddedMessage(newTask));
            
            this.Close();
        }
    }
}