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

            // Populate Year Picker with a range of years (e.g., current year � 50)
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

        private void OnMonthOrYearChanged(object? sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                // Ensure YearPicker.SelectedItem is not null
                if (YearPicker.SelectedItem != null)
                {
                    int selectedYear = int.Parse(YearPicker.SelectedItem.ToString()!); // The "!" asserts non-nullability
                    int selectedMonth = MonthPicker.SelectedIndex + 1;
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
            this.IsVisible = false;
        }

        // Method to show alert using the parent page
        private void ShowAlert(string title, string message)
        {
            _taskPage?.DisplayAlert(title, message, "OK");
        }

        // Done button click handler
        private void OnDoneClicked(object sender, EventArgs e)
        {

            // Get the values from the user inputs
            string taskTitle = TaskTitleEntry.Text?.Trim() ?? string.Empty;
            string taskDescription = DescriptionEditor.Text?.Trim() ?? string.Empty;
            string selectedMonth = MonthPicker.SelectedItem?.ToString() ?? string.Empty;
            string selectedDay = DayPicker.SelectedItem?.ToString() ?? string.Empty;
            string selectedYear = YearPicker.SelectedItem?.ToString() ?? string.Empty;

            // Format the due date as "DUE MM/DD/YYYY"
            string dueDate = $"{selectedMonth}/{selectedDay}/{selectedYear}";

            // Get the session count
            int sessionCount = int.TryParse(SessionCountLabel.Text, out var result) ? result : 1;

            // Ensure all necessary fields are filled
            if (string.IsNullOrEmpty(taskTitle) || string.IsNullOrEmpty(taskDescription) || string.IsNullOrEmpty(dueDate))
            {
                // Handle error: Show a message that fields are missing
                ShowAlert("Error", "Please fill out all the fields before proceeding.");
                return;
            }

            // Create a new Border for the task
            var taskBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Colors.White,
                Padding = new Thickness(15),
            };


            // Add GestureRecognizer for the Tap event
            taskBorder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => _taskPage.OnTaskTapped(taskBorder, EventArgs.Empty))
            });


            // Create a VerticalStackLayout for the task details
            var taskStack = new VerticalStackLayout();

            // Add the task labels
            taskStack.Children.Add(new Label
            {
                Text = $"DUE {dueDate}", // Example date
                FontSize = 16,
                TextColor = Colors.Black
            });

            taskStack.Children.Add(new Label
            {
                Text = TaskTitleEntry.Text ?? "No title", // Example task title
                FontSize = 16,
                TextColor = Colors.Black
            });

            taskStack.Children.Add(new Label
            {
                Text = DescriptionEditor.Text ?? "No description", // Description, set IsVisible to False initially
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            taskStack.Children.Add(new Label
            {
                Text = $"Number of Sessions: {sessionCount}", // Example session info
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            // Set the taskStack as the content of the Border
            taskBorder.Content = taskStack;

            // Add the taskBorder to the ExistingTasksContent
            _taskPage.AddNewTask(taskBorder);

            // Optionally hide the TaskPopUp after adding the task
            this.IsVisible = false;

            TaskCreated?.Invoke(this, EventArgs.Empty);
            // Hide the popup
            Hide();
        }
    }
}