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

    private void Task_Clicked(object sender, EventArgs e)
    {
        ChoosingTask.IsVisible = false;
        ChoosingPomodoro.IsVisible = true;
    }

    private void PomodoroCancel_Clicked(object sender, EventArgs e)
    {
        ChoosingPomodoro.IsVisible = false;
        ChoosingTask.IsVisible = true;
    }
}

