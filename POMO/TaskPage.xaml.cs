namespace POMO
{
    public partial class TaskPage : ContentPage
    {
        public TaskPage()
        {
            InitializeComponent();

        }

        private void OnCreateNewTaskClicked(object sender, EventArgs e)
        {

        }

        private void OnExistingTasksToggleClicked(object sender, EventArgs e)
        {

        }

        private void OnCompletedTasksToggleClicked(object sender, EventArgs e)
        {

        }

        private async void GoToHome(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("MainPage");
        }

        private async void GoToTimer(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("TimerPage");
        }

        private void OnTaskTapped(object sender, TappedEventArgs e)
        {

        }
    }
}