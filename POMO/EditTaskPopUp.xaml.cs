using Microsoft.Maui.Controls.Shapes;
using Microsoft.VisualBasic;

namespace POMO
{
    public partial class EditTaskPopUp : ContentView
	{
        public EditTaskPopUp()
		{
			InitializeComponent();

            // Populate Month Picker
            MonthPicker.ItemsSource = Enumerable.Range(1, 12).Select(m => m.ToString("D2")).ToList(); // "01" to "12"

            // Populate Year Picker with a range of years (e.g., current year ± 50)
            int currentYear = DateTime.Now.Year;
            YearPicker.ItemsSource = Enumerable.Range(currentYear, 6).Select(y => y.ToString()).ToList();

            // Populate Day Picker initially (default to 31 days)
            UpdateDayPicker(31);

            // Event to update days when month or year changes
            MonthPicker.SelectedIndexChanged += OnMonthOrYearChanged;
            YearPicker.SelectedIndexChanged += OnMonthOrYearChanged;
        }

        // Public properties to access UI elements
        public Entry TaskTitleEntryControl => TaskTitleEntry;
        public Editor DescriptionEditorControl => DescriptionEditor;
        public Picker MonthPickerControl => MonthPicker;
        public Picker DayPickerControl => DayPicker;
        public Picker YearPickerControl => YearPicker;
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

        private void OnMonthOrYearChanged(object? sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                // Ensure YearPicker.SelectedItem is not null
                if (YearPicker.SelectedItem != null)
                {
                    int selectedYear = YearPicker.SelectedItem != null
                        ? int.Parse(YearPicker.SelectedItem.ToString()!)
                        : DateTime.Now.Year; // Default to current year

                    int selectedMonth = MonthPicker.SelectedIndex != -1
                        ? MonthPicker.SelectedIndex + 1
                        : DateTime.Now.Month; // Default to current month

                    int daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
                    UpdateDayPicker(daysInMonth);
                }
            }
        }

        private void UpdateDayPicker(int days)
        {
            DayPicker.ItemsSource = Enumerable.Range(1, days).Select(d => d.ToString("D2")).ToList(); // "01" to "31"
        }

        // Event handler when the Cancel button is clicked
        private void OnCancelClicked(object sender, EventArgs e)
        {
            this.IsVisible = false;
        }

        public event Action<string, string, DateTime, int>? TaskUpdated;

        private async void OnDoneClicked(object sender, EventArgs e)
        {
            // Get updated values from the UI controls
            string updatedTitle = TaskTitleEntryControl.Text;
            string updatedDescription = DescriptionEditorControl.Text;

            if (MonthPickerControl.SelectedItem == null || DayPickerControl.SelectedItem == null || YearPickerControl.SelectedItem == null)
            {
                // Handle missing date selection
                ShowAlert("Error", "Please select a valid date.");
                return;
            }

            // Get the updated due date from the pickers
            int selectedMonth = int.Parse(MonthPickerControl.SelectedItem.ToString()!);
            int selectedDay = int.Parse(DayPickerControl.SelectedItem.ToString()!);
            int selectedYear = int.Parse(YearPickerControl.SelectedItem.ToString()!);

            // Check if the selected values form a valid date
            if (selectedMonth <= 0 || selectedMonth > 12 || selectedDay <= 0 || selectedDay > 31 || selectedYear <= 0)
            {
                ShowAlert("Error", "Please select a valid date.");
                return;
            }

            DateTime updatedDueDate = new DateTime(selectedYear, selectedMonth, selectedDay);

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
            this.IsVisible = false;
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