using Microsoft.VisualBasic;

namespace POMO
{
    public partial class TaskPage : ContentPage
    {
        private bool isExistingTasksVisible = true;
        private bool isCompletedTasksVisible = true;

        public EditTaskPopUp EditTaskPopUpInstance => this.FindByName<EditTaskPopUp>("EditTaskPopUp");
        public DeleteTaskPopUp DeleteTaskPopUpInstance => this.FindByName<DeleteTaskPopUp>("DeleteTaskPopUp");

        public TaskPage()
        {
            InitializeComponent();

            TaskPopUp.Initialize(this);

            // Access the TaskPopUp instance from XAML and subscribe to the events
            TaskPopUp.TaskCreated += OnTaskCreated;
            TaskPopUp.Cancelled += OnTaskCancelled;
            SpecificTaskPopUp.EditRequested += OnEditRequested;
            // Subscribe to the TaskUpdated event from EditTaskPopUp
            EditTaskPopUpInstance.TaskUpdated += OnTaskUpdated;
        }

        private void OnTaskUpdated(string updatedTitle, string updatedDescription, DateTime updatedDueDate, int updatedNumSessions)
        {
            if (selectedTaskBorder == null)
                return;

            // Extract the Grid from the selected task
            var grid = selectedTaskBorder.Content as Grid;
            if (grid == null)
                return;

            // Find the VerticalStackLayout within the Grid (Column 1)
            var taskDetailsStack = grid.Children
                .OfType<VerticalStackLayout>()
                .FirstOrDefault();

            if (taskDetailsStack == null)
                return;

            // Find the labels inside the stackLayout
            var dueDateLabel = taskDetailsStack.Children[0] as Label;
            var taskTitleLabel = taskDetailsStack.Children[1] as Label;
            var descriptionLabel = taskDetailsStack.Children[2] as Label;
            var NumSessionLabel = taskDetailsStack.Children[3] as Label;

            if (dueDateLabel != null && taskTitleLabel != null && descriptionLabel != null && NumSessionLabel != null)
            {
                // Update the labels with the new values
                dueDateLabel.Text = updatedDueDate.ToString("DUE MM/dd/yyyy");
                taskTitleLabel.Text = updatedTitle;
                descriptionLabel.Text = updatedDescription;
                NumSessionLabel.Text = $"Number of Sessions: {updatedNumSessions}";
            }
        }

        // Populate the EditTaskPopUp with the current task details
        private void OnEditRequested(string title, string description, DateTime dueDate, int numSessions)
        {
            // Populate the task details in EditTaskPopUp
            EditTaskPopUp.TaskTitleEntryControl.Text = title;
            EditTaskPopUp.DescriptionEditorControl.Text = description;

            //dueDate = DateTime.Now;

            // Set the pickers for the due date
            EditTaskPopUp.MonthPickerControl.SelectedIndex = 0;
            EditTaskPopUp.DayPickerControl.SelectedIndex = 0;
            EditTaskPopUp.YearPickerControl.SelectedIndex = 0;

            // Set the session count
            EditTaskPopUp.SessionCountLabelControl.Text = numSessions > 0 ? numSessions.ToString() : "1";

            // Show the EditTaskPopUp
            EditTaskPopUp.IsVisible = true;
        }

        // Update the method in TaskPopUp.xaml.cs to reference ExistingTasksContent directly
        public void AddNewTask(Border taskBorder)
        {
            // Ensure ExistingTasksContent is directly accessible
            ExistingTasksContent.Children.Add(taskBorder);
        }

        public void AddCompletedTask(Border taskBorder)
        {
            // Ensure ExistingTasksContent is directly accessible
            CompletedTasksContent.Children.Add(taskBorder);
        }

        public new async Task DisplayAlert(string title, string message, string cancel)
        {
            // Use the inherited DisplayAlert from ContentPage by explicitly hiding it here
            await base.DisplayAlert(title, message, cancel);
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
            TaskPopUp.Show(); // Show the TaskPopUp when the button is clicked
        }

        // Store a reference to the currently selected task
        public Border? selectedTaskBorder;

        public void OnTaskTapped(object sender, EventArgs e)
        {
            // Get the tapped Border
            var border = sender as Border;
            if (border == null)
                return;

            // Store the reference to the tapped task (Border)
            selectedTaskBorder = border;

            // Extract task details (labels inside the Grid within the Border)
            var grid = border.Content as Grid;
            if (grid == null)
                return;

            // Get the VerticalStackLayout from Column 1 of the Grid
            var taskDetailsStack = grid.Children
                .OfType<VerticalStackLayout>()
                .FirstOrDefault();

            if (taskDetailsStack == null)
                return;

            var dueDateLabel = taskDetailsStack.Children[0] as Label;
            var taskTitleLabel = taskDetailsStack.Children[1] as Label;
            var descriptionLabel = taskDetailsStack.Children[2] as Label;
            var NumSessionLabel = taskDetailsStack.Children[3] as Label;

            if (dueDateLabel != null && taskTitleLabel != null && descriptionLabel != null && NumSessionLabel != null)
            {
                // Pass the task details and the border reference to SpecificTaskPopUp
                SpecificTaskPopUp.DisplayTaskDetails(
                    dueDateLabel.Text,
                    taskTitleLabel.Text,
                    descriptionLabel.Text,
                    NumSessionLabel.Text
                );

                // Show the popup
                SpecificTaskPopUp.IsVisible = true;
            }
        }

        // Event handler when a task is created (done button clicked)
        private void OnTaskCreated(object? sender, EventArgs e)
        {
            _ = DisplayAlert("Task Created", "A new task has been successfully created.", "OK");
            TaskPopUp.IsVisible = false; // Hide the popup after task is created
        }

        public void OnMarkAsDoneClicked(object sender, EventArgs e)
        {
            if (selectedTaskBorder == null)
                return;

            // Clone the selected task
            Border clonedTaskBorder = CloneBorder(selectedTaskBorder);

            // Add the cloned task to the CompletedTasksContent section
            CompletedTasksContent.Children.Add(clonedTaskBorder);

            // Remove the selectedTaskBorder from its current layout
            var parentLayout = selectedTaskBorder.Parent as Microsoft.Maui.Controls.Layout;
            if (parentLayout != null)
            {
                parentLayout.Children.Remove(selectedTaskBorder);
            }

            // Clear the selected task reference
            selectedTaskBorder = null;
        }

        private Border CloneBorder(Border originalBorder)
        {
            // Create a new Border
            Border clonedBorder = new Border
            {
                StrokeShape = originalBorder.StrokeShape,
                BackgroundColor = originalBorder.BackgroundColor,
                Padding = originalBorder.Padding,
            };

            // Copy GestureRecognizers
            foreach (var gestureRecognizer in originalBorder.GestureRecognizers)
            {
                clonedBorder.GestureRecognizers.Add(gestureRecognizer); // Add each recognizer
            }

            // Clone the Content (assuming it's a Grid with a specific structure)
            if (originalBorder.Content is Grid originalGrid)
            {
                // Clone the Grid
                Grid clonedGrid = new Grid
                {
                    ColumnDefinitions = originalGrid.ColumnDefinitions,
                    ColumnSpacing = originalGrid.ColumnSpacing,
                    Padding = originalGrid.Padding
                };

                // Clone the Image
                if (originalGrid.Children[0] is Image originalImage)
                {
                    Image clonedImage = new Image
                    {
                        Source = "check_icon.png",
                        VerticalOptions = originalImage.VerticalOptions,
                        HorizontalOptions = originalImage.HorizontalOptions,
                        HeightRequest = originalImage.HeightRequest,
                        WidthRequest = originalImage.WidthRequest
                    };
                    Grid.SetColumn(clonedImage, Grid.GetColumn(originalImage));
                    clonedGrid.Children.Add(clonedImage);
                }

                // Clone the VerticalStackLayout
                if (originalGrid.Children[1] is VerticalStackLayout originalStack)
                {
                    VerticalStackLayout clonedStack = new VerticalStackLayout
                    {
                        VerticalOptions = originalStack.VerticalOptions
                    };

                    int labelIndex = 0;

                    foreach (var child in originalStack.Children)
                    {
                        if (child is Label originalLabel)
                        {
                            // Clone Label
                            Label clonedLabel = new Label
                            {
                                Text = originalLabel.Text,
                                FontSize = originalLabel.FontSize,
                                TextColor = labelIndex == 0 ? Color.FromArgb("#30BFBF") : originalLabel.TextColor,
                                IsVisible = originalLabel.IsVisible
                            };
                            clonedStack.Children.Add(clonedLabel);
                            labelIndex++; // Increment the label index
                        }
                    }

                    Grid.SetColumn(clonedStack, Grid.GetColumn(originalStack));
                    clonedGrid.Children.Add(clonedStack);
                }

                // Set the cloned Grid as the Content of the cloned Border
                clonedBorder.Content = clonedGrid;
            }

            return clonedBorder;
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

        private async void GoToTimer(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TimerPage");
        }
    }
}