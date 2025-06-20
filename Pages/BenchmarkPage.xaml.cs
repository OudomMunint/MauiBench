using System.Collections.Generic;

namespace MauiBench.Pages;

public partial class BenchmarkPage : ContentPage
{
    public List<string> Results { get; set; } = new List<string>();

    int count = 0;

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
        ResultsLabel.Text = thisbenchmarkResults;
        HashingSpinner.IsRunning = false;
        HashingSpinner.IsVisible = false;

        hashBenchmark?.DisposeOf();
    }
}