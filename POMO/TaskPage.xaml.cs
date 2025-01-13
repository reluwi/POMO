using CommunityToolkit.Maui.Views;

namespace POMO
{
    public partial class TaskPage : ContentPage
    {
        public TaskPage()
        {
            InitializeComponent();

        }

        private async void OnCreateNewTaskClicked(object sender, EventArgs e)
        {
            this.ShowPopup(new CreateNewTaskPopUp());
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
            this.ShowPopup(new SpecificTaskPopUp());
        }

        private void CompleteTaskTapped(object sender, TappedEventArgs e)
        {
            this.ShowPopup(new CompletedTaskPopUp());
        }
    }
}