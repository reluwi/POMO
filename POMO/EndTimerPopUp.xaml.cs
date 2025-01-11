using CommunityToolkit.Maui.Views;

namespace POMO
{

    public partial class EndTimerPopUp : Popup
    {
        public EndTimerPopUp(string message)
        {
            InitializeComponent();
            MessageLabel.Text = message;
        }

        private void ContinueButton_Clicked(object sender, EventArgs e)
        {
            // Close the popup and pass a result to the calling page
            Close("Continue");
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}