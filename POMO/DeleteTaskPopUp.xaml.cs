using Microsoft.Maui.Controls.Compatibility;

namespace POMO
{
    public partial class DeleteTaskPopUp : ContentView
    {
        public DeleteTaskPopUp()
        {
            InitializeComponent();
        }

        private void OnNoButtonClicked(object sender, EventArgs e)
        {
            // Simply hide the DeleteTaskPopUp
            this.IsVisible = false;
        }

        private async void OnYesButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Traverse the visual tree to find TaskPage
                Element parent = this;

                while (parent != null)
                {
                    if (parent is TaskPage taskPage)
                    {
                        // Call the DeleteTaskAsync method in TaskPage
                        await taskPage.DeleteTaskAsync();

                        // Hide the DeleteTaskPopUp
                        this.IsVisible = false;

                        return;
                    }

                    // Traverse to the next parent
                    parent = parent.Parent;
                }

                // If TaskPage was not found, log a message (optional)
                Console.WriteLine("TaskPage not found in the visual tree.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnYesButtonClicked: {ex.Message}");
                var mainPage = Application.Current?.Windows[0]?.Page;
                if (mainPage != null)
                {
                    await mainPage.DisplayAlert("Error", "An error occurred while deleting the task.", "OK");
                }
            }
        }
    }
}