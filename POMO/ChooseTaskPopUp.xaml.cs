using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;

namespace POMO
{

    public partial class ChooseTaskPopUp : Popup
    {
        public ChooseTaskPopUp()
        {
            InitializeComponent();
        }

        public void PopulateTasks(IEnumerable<TaskModel> tasks)
        {
            ChooseTasksContent.Children.Clear();
            Console.WriteLine("Cleared existing children in ChooseTasksContent");

            foreach (var task in tasks)
            {
                try
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
                            new ColumnDefinition { Width = new GridLength(0.2, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) },
                        },
                        ColumnSpacing = 5,
                        Padding = new Thickness(5),
                    };

                    // Add an Image to the Grid (Column 0)
                    var taskImage = new Image
                    {
                        Source = "existing_task_logo.png",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        HeightRequest = 30,
                        WidthRequest = 30,
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
                        TextColor = Color.FromArgb("#F73467"),
                        FontSize = 15
                    });

                    taskDetailsStack.Children.Add(new Label
                    {
                        Text = task.Title ?? "No title", // Example task title
                        TextColor = Colors.Black,
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold
                    });

                    // Add the VerticalStackLayout to the Grid (Column 1)
                    taskGrid.Add(taskDetailsStack, 1, 0);

                    // Set the taskStack as the content of the Border
                    taskBorder.Content = taskGrid;

                    // Add the taskBorder to ChooseTasksContent
                    ChooseTasksContent.Children.Add(taskBorder);

                    Console.WriteLine($"Added task: {task.Title}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error adding task: {ex.Message}");
                }
            }
        }

        public async void LoadTasksFromTaskPage()
        {
            try
            {
                var tasks = await App.Database.GetTasksAsync();
                if (tasks == null || !tasks.Any())
                {
                    Console.WriteLine("No tasks found in the database");
                    tasks = new List<TaskModel>(); // Initialize tasks to an empty list to avoid null reference
                }
                else
                {
                    Console.WriteLine($"Found {tasks.Count} tasks in the database");
                }

                // Filter out tasks that are marked as done
                var filteredTasks = tasks.Where(task => !task.IsCompleted).ToList();
                Console.WriteLine($"Filtered tasks count: {filteredTasks.Count}");

                PopulateTasks(filteredTasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks from database: {ex.Message}");
            }
        }

        private void OnTaskTapped(object sender, EventArgs e)
        {
            if (sender is Border taskBorder && taskBorder.BindingContext is TaskModel task)
            {
                // Get the title and number of sessions of the selected task
                var taskTitle = task.Title ?? "No title";
                var numSessions = task.NumSessions;
                var completedSessions = task.CompletedSessions;

                // Format the task information
                var taskInfo = $"{taskTitle} ({completedSessions} / {numSessions})";

                // Pass the task ID and task information back to TimerPage
                Close(new Tuple<int, string, int>(task.Id, taskInfo, completedSessions));
            }
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }

    }
}