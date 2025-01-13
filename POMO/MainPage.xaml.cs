using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls.Shapes;

namespace POMO
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load tasks due today
            LoadTasksDueToday();
        }

        private async void LoadTasksDueToday()
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

                // Filter tasks that are due today
                var tasksDueToday = tasks.Where(task => task.DueDate.Date == DateTime.Today).ToList();
                Console.WriteLine($"Tasks due today count: {tasksDueToday.Count}");

                // Populate the DueTasksContent with tasks due today
                PopulateTasksDueToday(tasksDueToday);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks from database: {ex.Message}");
            }
        }

        private void PopulateTasksDueToday(IEnumerable<TaskModel> tasks)
        {
            DueTasksContent.Children.Clear();

            if (!tasks.Any())
            {
                // Add a message if no tasks are due today
                DueTasksContent.Children.Add(new Border
                {
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    BackgroundColor = Colors.White,
                    Padding = new Thickness(15),
                    Content = new Label
                    {
                        Text = "No deadlines today—enjoy! 😊",
                        TextColor = Colors.Black,
                        FontSize = 16
                    }
                });
            }
            else
            {
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
                                new ColumnDefinition { Width = new GridLength(0.8, GridUnitType.Star) },
                                new ColumnDefinition { Width = new GridLength(0.2, GridUnitType.Star) },
                            },
                            ColumnSpacing = 5,
                            Padding = new Thickness(5),
                        };

                        // Create a VerticalStackLayout for task details (Column 1)
                        var taskDetailsStack = new VerticalStackLayout
                        {
                            VerticalOptions = LayoutOptions.Center
                        };

                        taskDetailsStack.Children.Add(new Label
                        {
                            Text = task.Title ?? "No title", // Example task title
                            TextColor = Colors.Black,
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold
                        });

                        // Add the task labels
                        taskDetailsStack.Children.Add(new Label
                        {
                            Text = "DUE TODAY", // Example date
                            TextColor = Color.FromArgb("#F73467"),
                            FontSize = 13
                        });

                        // Add the VerticalStackLayout to the Grid (Column 1)
                        taskGrid.Add(taskDetailsStack, 0, 0);

                        // Set the taskStack as the content of the Border
                        taskBorder.Content = taskGrid;

                        // Add the taskBorder to DueTasksContent
                        DueTasksContent.Children.Add(taskBorder);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error adding task: {ex.Message}");
                    }
                }
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

                // Send the TaskSelectedMessage
                WeakReferenceMessenger.Default.Send(new DueTaskSelectedMessage((task.Id, taskTitle, completedSessions, numSessions)));

                // Navigate to TimerPage
                Shell.Current.GoToAsync("TimerPage");
            }
        }

        private async void OnTaskButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TaskPage
            await Shell.Current.GoToAsync("TaskPage");
        }

        private async void GoToTimer(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TimerPage");
        }
    }

}
