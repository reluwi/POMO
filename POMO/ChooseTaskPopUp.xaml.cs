using CommunityToolkit.Maui.Views;

namespace POMO
{

	public partial class ChooseTaskPopUp : Popup
    {
		public ChooseTaskPopUp()
		{
			InitializeComponent();
		}

        private void Task_Clicked(object sender, EventArgs e)
        {
            // Example: Return a task name or identifier to the calling page
            Close("Activity 2");
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }

    }
}