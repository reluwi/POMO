using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.VisualBasic;

namespace POMO
{
    public partial class TaskPopUp : ContentView
    {
        // Define events for Task creation and cancellation
        public event EventHandler? TaskCreated;
        public event EventHandler? Cancelled;

        private TaskPage? _taskPage;

        public TaskPopUp()
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
            Cancelled?.Invoke(this, EventArgs.Empty); // Trigger the Cancel event
        }

        // Method to hide the pop-up
        public void Hide()
        {
            TaskTitleEntryControl.Text = string.Empty;
            DescriptionEditorControl.Text = string.Empty;
            SessionCountLabelControl.Text = "1";

            // Reset due date pickers
            MonthPicker.SelectedIndex = -1;         
            DayPicker.SelectedIndex = -1;           
            YearPicker.SelectedIndex = -1;           


            this.IsVisible = false;
        }

        public void Show()
        {
            TaskTitleEntryControl.Text = string.Empty;
            DescriptionEditorControl.Text = string.Empty;
            SessionCountLabelControl.Text = "1";

            // Reset due date pickers
            MonthPicker.SelectedIndex = 0;
            DayPicker.SelectedIndex = 0;
            YearPicker.SelectedIndex = 0;


            this.IsVisible = true;
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
            string selectedMonth = MonthPicker.SelectedItem?.ToString() ?? string.Empty;
            string selectedDay = DayPicker.SelectedItem?.ToString() ?? string.Empty;
            string selectedYear = YearPicker.SelectedItem?.ToString() ?? string.Empty;

            // Ensure all necessary fields are filled
            if (string.IsNullOrEmpty(taskTitle) || string.IsNullOrEmpty(taskDescription) || string.IsNullOrEmpty(selectedMonth) || string.IsNullOrEmpty(selectedDay) || string.IsNullOrEmpty(selectedYear))
            {
                // Handle error: Show a message that fields are missing
                ShowAlert("Error", "Please fill out all the fields before proceeding.");
                return;
            }

            // Construct the due date from the selected values
            DateTime dueDate = new DateTime(int.Parse(selectedYear), int.Parse(selectedMonth), int.Parse(selectedDay));

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

            // Add the task to the UI
            _taskPage?.AddTaskToUI(newTask);

            // Optionally hide the TaskPopUp after adding the task
            this.IsVisible = false;

            TaskCreated?.Invoke(this, EventArgs.Empty);
            // Hide the popup
            Hide();
        }
    }
}