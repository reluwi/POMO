namespace POMO
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        // Handle Sign Up button click
        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // Implement the sign-up logic here, e.g., validate inputs and register the user
            await DisplayAlert("Sign Up", "User signed up successfully!", "OK");
            // After successful sign-up, navigate back to LoginPage
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // Navigate to LoginPage when "Sign In" is tapped
        private async void OnSignInTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage"); // Go back to LoginPage
        }
    }
}