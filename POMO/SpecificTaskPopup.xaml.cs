using CommunityToolkit.Maui.Views;

namespace POMO;

public partial class SpecificTaskPopUp : Popup
{
	public SpecificTaskPopUp()
	{
		InitializeComponent();
	}

    private void CloseButton_Clicked(object sender, EventArgs e)
    {
		Close();
    }

    private void OnDescriptionTextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void OnDecreaseClicked(object sender, EventArgs e)
    {

    }

    private void OnIncreaseClicked(object sender, EventArgs e)
    {

    }
}