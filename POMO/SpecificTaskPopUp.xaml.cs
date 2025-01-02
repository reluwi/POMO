namespace POMO
{
	public partial class SpecificTaskPopUp : ContentView
	{
        public required string DueDate { get; set; }
        public required string TaskTitle { get; set; }
        public required string Description { get; set; }
        public required string NumSession { get; set; }

        public SpecificTaskPopUp()
		{
			InitializeComponent();
        }

        public void DisplayTaskDetails(string dueDate, string taskTitle, string description, string numSession)
        {
            DueDate = dueDate;
            TaskTitle = taskTitle;
            Description = description;
            NumSession = numSession;

            DueDateLabel.Text = dueDate; // Example: XAML element with x:Name="DueDateLabel"
            TaskTitleLabel.Text = taskTitle; // Example: XAML element with x:Name="TaskTitleLabel"
            DescriptionLabel.Text = description;
            NumSessionLabel.Text = numSession;
            IsVisible = true;
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        public void Hide()
        {
            this.IsVisible = false;
        }

        private void OnEditTaskClicked(object sender, EventArgs e)
        {
            // Logic for editing the task
        }

        private void OnCloseClicked(object sender, EventArgs e)
        {
            Hide();
        }

    }
}