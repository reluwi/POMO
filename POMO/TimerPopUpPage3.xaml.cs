using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class TimerPopUpPage3 : Popup
{
    public TimerPopUpPage3()
    {
        InitializeComponent();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}