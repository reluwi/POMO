namespace POMO
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }


        private async void GoToTimer(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TimerPage");
        }

        private async void GoToTask(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TaskPage");
        }
    }

}
