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

        private void OnYesButtonClicked(object sender, EventArgs e)
        {
            // Traverse the visual tree to find TaskPage
            Element parent = this;

            while (parent != null)
            {
                if (parent is TaskPage taskPage)
                {
                    // Delete the selected task
                    if (taskPage.selectedTaskBorder != null)
                    {
                        var parentLayout = taskPage.selectedTaskBorder.Parent as Microsoft.Maui.Controls.Layout;
                        if (parentLayout != null)
                        {
                            parentLayout.Children.Remove(taskPage.selectedTaskBorder);
                        }
                    }

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
    }
}