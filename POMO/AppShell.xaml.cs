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
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            GoToAsync("//LoginPage");

            // Register SignUpPage route
            Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));

            // Register TimerPage route
            Routing.RegisterRoute("TimerPage", typeof(TimerPage));
        }
    }
}
