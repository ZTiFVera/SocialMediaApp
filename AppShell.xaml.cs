namespace SocialMediaApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // All routes are declared as ShellContent in AppShell.xaml.
            // No manual Routing.RegisterRoute calls needed — doing so would
            // throw a duplicate-route exception at runtime.
        }
    }
}