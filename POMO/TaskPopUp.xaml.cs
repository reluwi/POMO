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
            Cancelled?.Invoke(this, EventArgs.Empty); // Trigger the Cancel event
        }

        // Method to hide the pop-up
        public void Hide()
        {
            TaskTitleEntry.Text = string.Empty;
            DescriptionEditor.Text = string.Empty;
            SessionCountLabel.Text = "1";

            // Reset due date pickers
            MonthPicker.SelectedIndex = -1;         
            DayPicker.SelectedIndex = -1;           
            YearPicker.SelectedIndex = -1;           


            this.IsVisible = false;
        }

        public void Show()
        {
            TaskTitleEntry.Text = string.Empty;
            DescriptionEditor.Text = string.Empty;
            SessionCountLabel.Text = "1";

            // Reset due date pickers
            MonthPicker.SelectedIndex = -1;
            DayPicker.SelectedIndex = -1;
            YearPicker.SelectedIndex = -1;


            this.IsVisible = true;
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
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            taskBorder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => _taskPage.OnTaskTapped(taskBorder, EventArgs.Empty))
            });
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Create a Grid for the layout
            var taskGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
    {
                    new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) },
    },
                ColumnSpacing = 10,
                Padding = new Thickness(3),
            };

            // Add an Image to the Grid (Column 0)
            var taskImage = new Image
            {
                Source = "existing_task_logo.png",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 20,
                WidthRequest = 20,
            };
            taskGrid.Add(taskImage, 0, 0); // Add to Column 0

            // Create a VerticalStackLayout for the task details
            // Create a VerticalStackLayout for task details (Column 1)
            var taskDetailsStack = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center
            };

            // Add the task labels
            taskDetailsStack.Children.Add(new Label
            {
                Text = $"DUE {dueDate}", // Example date
                TextColor = Color.FromArgb("#F73467"),
                FontSize = 15
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = TaskTitleEntry.Text ?? "No title", // Example task title
                TextColor = Colors.Black,
                FontSize = 18
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = DescriptionEditor.Text ?? "No description", // Description, set IsVisible to False initially
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = $"Number of Sessions: {sessionCount}", // Example session info
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            // Add the VerticalStackLayout to the Grid (Column 1)
            taskGrid.Add(taskDetailsStack, 1, 0);

            // Set the taskStack as the content of the Border
            taskBorder.Content = taskGrid;

            // Add the taskBorder to the ExistingTasksContent
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            _taskPage.AddNewTask(taskBorder);
            #pragma warning restore CS8602 // Dereference of a possibly null reference.

            // Optionally hide the TaskPopUp after adding the task
            this.IsVisible = false;

            TaskCreated?.Invoke(this, EventArgs.Empty);
            // Hide the popup
            Hide();
        }
    }
}