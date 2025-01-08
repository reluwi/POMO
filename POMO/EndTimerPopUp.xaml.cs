using CommunityToolkit.Maui.Views;

namespace POMO
{

    public partial class EndTimerPopUp : Popup
    {
        public EndTimerPopUp()
        {
            InitializeComponent();
        }

        private void CloseButton_Clicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}