using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class TimerPopUpPage2 : Popup
{
    public TimerPopUpPage2()
    {
        InitializeComponent();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void Task_Clicked(object sender, ElementEventArgs e)
    {
        Close();

        TimerPage ChooseTask = new TimerPage();
        ChooseTask.ShowPopup();
    }

}