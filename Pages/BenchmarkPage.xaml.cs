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
    }

    private async void PrimeBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        PrimeSpinner.IsRunning = true;
        PrimeSpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.CPUBenchmark? cpuBenchmark = null;

        await Task.Run(() =>
        {
            cpuBenchmark = new Benchmarks.CPUBenchmark();
            thisbenchmarkResults = cpuBenchmark.CpuPrimeCompute();
        });

        Results.Add(thisbenchmarkResults);
        PrimeResultsLabel.Text = thisbenchmarkResults;
        PrimeSpinner.IsRunning = false;
        PrimeSpinner.IsVisible = false;
    }

    private async void MatrixBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        MatrixSpinner.IsRunning = true;
        MatrixSpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.MatrixMultiplicationBenchmark? matrixBenchmark = null;

        await Task.Run(() =>
        {
            matrixBenchmark = new Benchmarks.MatrixMultiplicationBenchmark();
            thisbenchmarkResults = matrixBenchmark.MultiplyMatrix();
        });

        Results.Add(thisbenchmarkResults);
        MatrixResultsLabel.Text = thisbenchmarkResults;
        MatrixSpinner.IsRunning = false;
        MatrixSpinner.IsVisible = false;
    }

    private async void MemoryBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        bool isST = false; // WIP

        MemorySpinner.IsRunning = true;
        MemorySpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.MemoryBenchmark? memoryBenchmark = null;

        if (!isST)
        {
            await Task.Run(() => {
                memoryBenchmark = new Benchmarks.MemoryBenchmark();
                thisbenchmarkResults = memoryBenchmark.MTMemBandwidth();
            });
        }
        else
        {
            await Task.Run(() => {
                memoryBenchmark = new Benchmarks.MemoryBenchmark();
                thisbenchmarkResults = memoryBenchmark.STMemBandwidth();
            });
        }

        Results.Add(thisbenchmarkResults);
        MemoryResultsLabel.Text = thisbenchmarkResults;
        MemorySpinner.IsRunning = false;
        MemorySpinner.IsVisible = false;
    }
}