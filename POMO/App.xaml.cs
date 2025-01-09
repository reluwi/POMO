
using System.IO;


namespace POMO
{
    public partial class App : Application
    {
        static TaskDatabase? database;
        public static TaskDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new TaskDatabase(Path.Combine(FileSystem.AppDataDirectory, "Tasks.db3"));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}