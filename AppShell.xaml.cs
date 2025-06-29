namespace MauiBench
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            if (OperatingSystem.IsWindows())
            {
                BenchmarkPage.Icon = "speedom.png";
                ResultsPage.Icon = "results.png";
                SystemInfo.Icon = "info.png";
            }
            else
            {
                BenchmarkPage.Icon = null;
                ResultsPage.Icon = null;
                SystemInfo.Icon = null;
            }
        }
    }
}