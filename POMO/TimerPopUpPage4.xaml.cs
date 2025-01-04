using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class TimerPopUpPage4 : Popup
{
    public TimerPopUpPage4()
    {
        InitializeComponent();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}