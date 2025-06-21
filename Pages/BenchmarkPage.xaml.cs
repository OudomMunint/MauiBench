using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBench.Pages;

public partial class BenchmarkPage : ContentPage
{
    public List<string> Results { get; set; } = new List<string>();

    public BenchmarkPage()
    {
        InitializeComponent();
    }

    private async void HashingBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        HashingSpinner.IsRunning = true;
        HashingSpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.HashingBenchmark? hashBenchmark = null;

        await Task.Run(() =>
        {
            hashBenchmark = new Benchmarks.HashingBenchmark();
            thisbenchmarkResults = hashBenchmark.CombinedHashingExport();
        });

        Results.Add(thisbenchmarkResults);
        HashingResultsLabel.Text = thisbenchmarkResults;
        HashingSpinner.IsRunning = false;
        HashingSpinner.IsVisible = false;

        hashBenchmark?.DisposeOf();
    }

    private async void EncryptionBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        EncryptionSpinner.IsRunning = true;
        EncryptionSpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.EncryptionBenchmark? encBenchmark = null;

        await Task.Run(() =>
        {
            encBenchmark = new Benchmarks.EncryptionBenchmark();
            thisbenchmarkResults = encBenchmark.RunEncryptBenchmark();
        });

        Results.Add(thisbenchmarkResults);
        EncryptionResultsLabel.Text = thisbenchmarkResults;
        EncryptionSpinner.IsRunning = false;
        EncryptionSpinner.IsVisible = false;

        encBenchmark?.Dispose();
    }
}