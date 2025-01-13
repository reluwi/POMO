using Microsoft.Maui.Controls.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Maui.Views;

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

            SpecificTaskPopUp.EditRequested += OnEditRequested;
            EditTaskPopUpInstance.TaskUpdated += OnTaskUpdated;

            // Register to listen for TaskAddedMessage
            WeakReferenceMessenger.Default.Register<TaskAddedMessage>(this, (r, message) =>
            {
                AddTaskToUI(message.Task);
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var tasks = await App.Database.GetTasksAsync();
            foreach (var task in tasks)
            {
                AddTaskToUI(task);
            }
            
        }

        public List<TaskModel> GetExistingTasks()
        {
            return ExistingTasksContent.Children
                .OfType<Border>()
                .Select(c => (TaskModel)c.BindingContext)
                .ToList();
        }

        public void AddTaskToUI(TaskModel task)
        {
            // Create a new Border for the task
            var taskBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                BackgroundColor = Colors.White,
                Padding = new Thickness(15),
                BindingContext = task // Set the BindingContext to the TaskModel
            };

            taskBorder.GestureRecognizers.Clear(); // Clear any old gesture recognizers
                                                   // Add GestureRecognizer for the Tap event
            taskBorder.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnTaskTapped(taskBorder, EventArgs.Empty))
            });

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
                Source = task.IsCompleted ? "check_icon.png" : "existing_task_logo.png",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 20,
                WidthRequest = 20,
            };
            taskGrid.Add(taskImage, 0, 0); // Add to Column 0

            // Create a VerticalStackLayout for task details (Column 1)
            var taskDetailsStack = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center
            };

            // Add the task labels
            taskDetailsStack.Children.Add(new Label
            {
                Text = $"DUE {task.DueDate:MM/dd/yyyy}", // Example date
                TextColor = task.IsCompleted ? Color.FromArgb("#30BFBF") : Color.FromArgb("#F73467"),
                FontSize = 15
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = task.Title ?? "No title", // Example task title
                TextColor = Colors.Black,
                FontSize = 18
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = task.Description ?? "No description", // Description, set IsVisible to False initially
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            taskDetailsStack.Children.Add(new Label
            {
                Text = $"Number of Sessions: {task.NumSessions}", // Example session info
                FontSize = 16,
                TextColor = Colors.Black,
                IsVisible = false
            });

            // Add the VerticalStackLayout to the Grid (Column 1)
            taskGrid.Add(taskDetailsStack, 1, 0);

            // Set the taskStack as the content of the Border
            taskBorder.Content = taskGrid;

            // Add the taskBorder to the appropriate container
            if (task.IsCompleted)
            {
                CompletedTasksContent.Children.Add(taskBorder);
            }
            else
            {
                ExistingTasksContent.Children.Add(taskBorder);
            }
        }

        private async void OnTaskUpdated(string updatedTitle, string updatedDescription, DateTime updatedDueDate, int updatedNumSessions)
        {
            if (selectedTaskBorder == null)
                return;

            var task = (TaskModel)selectedTaskBorder.BindingContext;
            task.Title = updatedTitle;
            task.Description = updatedDescription;
            task.DueDate = updatedDueDate;
            task.NumSessions = updatedNumSessions;

            await App.Database.SaveTaskAsync(task);
            UpdateTaskUI(task);
        }

        public void UpdateTaskUI(TaskModel task)
        {
            if (selectedTaskBorder == null)
                return;

            // Extract task details (labels inside the Grid within the Border)
            var grid = selectedTaskBorder.Content as Grid;
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
            var numSessionLabel = taskDetailsStack.Children[3] as Label;

            if (dueDateLabel != null)
            {
                dueDateLabel.Text = $"DUE {task.DueDate:MM/dd/yyyy}";
            }

            if (taskTitleLabel != null)
            {
                taskTitleLabel.Text = task.Title ?? "No title";
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.Text = task.Description ?? "No description";
            }

            if (numSessionLabel != null)
            {
                numSessionLabel.Text = $"Number of Sessions: {task.NumSessions}";
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
            var taskPopUp = new TaskPopUp();
            taskPopUp.Initialize(this);
            this.ShowPopup(taskPopUp);
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

            // Extract the TaskModel from the BindingContext
            var task = border.BindingContext as TaskModel;
            if (task == null)
                return;

            // Set the task in SpecificTaskPopUp
            SpecificTaskPopUp.SetTask(task);

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

                // Check if the tapped task is under CompletedTasksContent
                if (CompletedTasksContent.Children.Contains(border))
                {
                    // Hide the "Mark as Done" button in SpecificTaskPopUp
                    SpecificTaskPopUp.HideButtons();
                }
                else
                {
                    // Show the "Mark as Done" button
                    SpecificTaskPopUp.ShowButtons();
                }

                // Show the popup
                SpecificTaskPopUp.IsVisible = true;
            }
        }

        public async Task DeleteTaskAsync()
        {
            if (selectedTaskBorder == null)
            {
                Console.WriteLine("No task selected for deletion.");
                return;
            }

            try
            {
                var task = (TaskModel)selectedTaskBorder.BindingContext;

                if (task == null)
                {
                    Console.WriteLine("Task binding context is null.");
                    return;
                }

                // Delete the task from the database
                int rowsAffected = await App.Database.DeleteTaskAsync(task);
                Console.WriteLine($"Task with ID {task.Id} deleted from database. Rows affected: {rowsAffected}");

                // Remove the task from the UI
                if (task.IsCompleted)
                {
                    CompletedTasksContent.Children.Remove(selectedTaskBorder);
                }
                else
                {
                    ExistingTasksContent.Children.Remove(selectedTaskBorder);
                }
                Console.WriteLine("Task removed from UI.");

                // Clear the selected task reference
                selectedTaskBorder = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteTaskAsync: {ex.Message}");
                var mainPage = Application.Current?.Windows[0]?.Page;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", "An error occurred while deleting the task.", "OK");
                }
            }
        }

        public async void OnMarkAsDoneClicked(object sender, EventArgs e)
        {
            if (selectedTaskBorder == null)
                return;

            if (selectedTaskBorder.Parent == ExistingTasksContent)
            {
                var task = (TaskModel)selectedTaskBorder.BindingContext;

                if (task == null)
                {
                    Console.WriteLine("Task binding context is null.");
                    return;
                }

                // Mark the task as completed
                task.IsCompleted = true;

                // Remove from ExistingTasksContent
                ExistingTasksContent.Children.Remove(selectedTaskBorder);

                // Add to CompletedTasksContent
                CompletedTasksContent.Children.Add(selectedTaskBorder);

                // Update design for completed task
                if (selectedTaskBorder.Parent == CompletedTasksContent)
                {
                    // Access the taskGrid (the Grid inside the taskBorder)
                    var taskGrid = selectedTaskBorder.Content as Grid;
                    if (taskGrid != null)
                    {
                        // Access the Image inside the taskGrid (Column 0)
                        var taskImage = taskGrid.Children.OfType<Image>().FirstOrDefault();
                        if (taskImage != null)
                        {
                            taskImage.Source = "check_icon.png"; // Change to check icon for completed task
                        }

                        // Access the taskDetailsStack (VerticalStackLayout inside the taskGrid)
                        var taskDetailsStack = taskGrid.Children.OfType<VerticalStackLayout>().FirstOrDefault();
                        if (taskDetailsStack != null)
                        {
                            // Access the Due Date label inside taskDetailsStack
                            var dueDateLabel = taskDetailsStack.Children.OfType<Label>().FirstOrDefault(label => label.Text.StartsWith("DUE"));
                            if (dueDateLabel != null)
                            {
                                dueDateLabel.TextColor = Color.FromArgb("#30BFBF"); // Set color for completed task
                            }
                        }
                    }
                }

                // Save the changes to the database
                await App.Database.SaveTaskAsync(task);
            }
            else
            {
                Console.WriteLine("Task is already in CompletedTasksContent");
            }

            // Clear the selected task reference
            selectedTaskBorder = null;

            // Log the completion status
            Console.WriteLine("Task moved to CompletedTasksContent");
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