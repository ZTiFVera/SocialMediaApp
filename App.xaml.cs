namespace SocialMediaApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // CreateWindow replaces the deprecated MainPage setter (CS0618)
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}

