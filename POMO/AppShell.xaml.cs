namespace POMO
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register MainPage route
            Routing.RegisterRoute("MainPage", typeof(MainPage));

            // Navigate to LoginPage as the first page
            Routing.RegisterRoute("LoginPage", typeof(StartingPage));
            GoToAsync("//StartingPage");

            // Register TimerPage route
            Routing.RegisterRoute("TimerPage", typeof(TimerPage));

            // Register TaskPage route
            Routing.RegisterRoute("TaskPage", typeof(TaskPage));
        }
    }
}
