using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class CompletedTaskPopUp : Popup
{
    public CompletedTaskPopUp()
    {
        InitializeComponent();
    }

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}