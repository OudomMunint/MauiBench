namespace MauiBench
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}