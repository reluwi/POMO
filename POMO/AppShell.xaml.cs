namespace POMO
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Navigate to LoginPage as the first page
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            GoToAsync("//LoginPage");
        }
    }
}
