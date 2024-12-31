namespace POMO
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnTaskButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TaskPage
            await Shell.Current.GoToAsync("TaskPage");
        }
    }

}
