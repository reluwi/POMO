using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class TimerPopUpPage1 : Popup
{
	public TimerPopUpPage1()
	{
		InitializeComponent();
	}

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
		Close();
    }
}