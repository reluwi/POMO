namespace POMO
{
	public partial class TaskPopUp : ContentView
	{
        // Define events for Task creation and cancellation
        public event EventHandler? TaskCreated;
        public event EventHandler? Cancelled;
        public TaskPopUp()
		{
			InitializeComponent();
		}

        // Event handler when the Cancel button is clicked
        private void OnCancelClicked(object sender, EventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty); // Trigger the Cancel event
        }

        // Event handler when the Done button is clicked
        private void OnDoneClicked(object sender, EventArgs e)
        {
            TaskCreated?.Invoke(this, EventArgs.Empty); // Trigger the TaskCreated event
        }
    }

}