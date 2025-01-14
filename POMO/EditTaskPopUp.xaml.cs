using Microsoft.Maui.Controls.Shapes;
using Microsoft.VisualBasic;
using CommunityToolkit.Maui.Views;

namespace POMO
{
    public partial class EditTaskPopUp : Popup
	{
        public EditTaskPopUp()
		{
			InitializeComponent();

            // Set the default due date to the current date
            DatePickerControl.Date = DateTime.Now;
        }

        // Public properties to access UI elements
        public Entry TaskTitleEntryControl => TaskTitleEntry;
        public Editor DescriptionEditorControl => DescriptionEditor;
        public DatePicker DatePickerControl => DatePickerEditor;
        public Label SessionCountLabelControl => SessionCountLabel;


        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            // Parse the current session count
            int currentCount = int.Parse(SessionCountLabel.Text);

            // Increment the session count
            currentCount++;

            // Update the label
            SessionCountLabel.Text = currentCount.ToString();
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            // Parse the current session count
            int currentCount = int.Parse(SessionCountLabel.Text);

            // Decrement the session count only if it's greater than 1
            if (currentCount > 1)
            {
                currentCount--;
            }

            // Update the label
            SessionCountLabel.Text = currentCount.ToString();
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
            this.Close();
        }

        public event Action<string, string, DateTime, int>? TaskUpdated;

        private async void OnDoneClicked(object sender, EventArgs e)
        {
            // Get updated values from the UI controls
            string updatedTitle = TaskTitleEntryControl.Text;
            string updatedDescription = DescriptionEditorControl.Text;
            // Construct the due date from the selected values
            DateTime dueDate = DatePickerControl.Date;

            DateTime updatedDueDate = dueDate;

            // Get the updated session count
            int updatedNumSessions = int.Parse(SessionCountLabelControl.Text);

            // Ensure all necessary fields are filled
            if (string.IsNullOrEmpty(updatedTitle) || string.IsNullOrEmpty(updatedDescription) || updatedDueDate == default(DateTime))
            {
                // Handle error: Show a message that fields are missing
                ShowAlert("Error", "Please fill out all the fields before proceeding.");
                return;
            }

            // Trigger the event to pass the updated values to TaskPage
            TaskUpdated?.Invoke(updatedTitle, updatedDescription, updatedDueDate, updatedNumSessions);

            // Save the updated task to the database
            if (this.Parent is TaskPage taskPage && taskPage.selectedTaskBorder != null)
            {
                var task = (TaskModel)taskPage.selectedTaskBorder.BindingContext;
                task.Title = updatedTitle;
                task.Description = updatedDescription;
                task.DueDate = updatedDueDate;
                task.NumSessions = updatedNumSessions;

                await App.Database.SaveTaskAsync(task);
                taskPage.UpdateTaskUI(task);
            }

            // Hide the EditTaskPopUp after updating
            this.Close();
        }

        private void ShowAlert(string title, string message)
        {
            if (this.Parent is TaskPage taskPage)
            {
                _ = taskPage.DisplayAlert(title, message, "OK");
            }
            else
            {
                // Fallback to handle unexpected case
                Console.WriteLine("Error: TaskPage not found.");
            }
        }
    }
}