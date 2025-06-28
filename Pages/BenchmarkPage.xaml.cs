using MauiBench.Data;
using MauiBench.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBench.Pages;

public partial class BenchmarkPage : ContentPage
{
    public List<int> Results { get; set; } = new List<int>();

    public ItemDatabase database { get; private set; } = null!;

    public BenchmarkPage()
    {
        InitializeComponent();
        InitializeDatabaseAsync();
        SetVersionAndTitle();
    }

    private async void InitializeDatabaseAsync()
    {
        database = await ItemDatabase.Instance;
    }

    private void SetVersionAndTitle()
    {
        TitleLabel.Text = $"MauiBench v{VersionTracking.Default.CurrentVersion}";
    }

    private async void HashingBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running Hashing benchmark...";
        }

        HashingSpinner.IsRunning = true;
        HashingSpinner.IsVisible = true;

        var thisbenchmarkResults = 0;
        Benchmarks.HashingBenchmark? hashBenchmark = null;

        await Task.Run(() =>
        {
            hashBenchmark = new Benchmarks.HashingBenchmark();
            thisbenchmarkResults = hashBenchmark.CombinedHashingExport();
        });

        Results.Add(thisbenchmarkResults);
        HashingResultsLabel.Text = $"{thisbenchmarkResults} points";
        HashingSpinner.IsRunning = false;
        HashingSpinner.IsVisible = false;

        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }

        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.Hashing,
            BenchmarkType = BenchmarkModel.Type.Full,
            Result = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }

    private async void EncryptionBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running Encryption benchmark...";
        }

        EncryptionSpinner.IsRunning = true;
        EncryptionSpinner.IsVisible = true;

        var thisbenchmarkResults = 0;
        Benchmarks.EncryptionBenchmark? encBenchmark = null;

        await Task.Run(() =>
        {
            encBenchmark = new Benchmarks.EncryptionBenchmark();
            thisbenchmarkResults = encBenchmark.RunEncryptBenchmark();
        });

        Results.Add(thisbenchmarkResults);
        EncryptionResultsLabel.Text = $"{thisbenchmarkResults} points";
        EncryptionSpinner.IsRunning = false;
        EncryptionSpinner.IsVisible = false;

        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }

        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.Encryption,
            BenchmarkType = BenchmarkModel.Type.Partial,
            Result = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }

    private async void PrimeBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running Prime benchmark...";
        }

        PrimeSpinner.IsRunning = true;
        PrimeSpinner.IsVisible = true;

        var thisbenchmarkResults = 0;
        Benchmarks.CPUBenchmark? cpuBenchmark = null;

        await Task.Run(() =>
        {
            cpuBenchmark = new Benchmarks.CPUBenchmark();
            thisbenchmarkResults = cpuBenchmark.CpuPrimeCompute();
        });

        Results.Add(thisbenchmarkResults);
        PrimeResultsLabel.Text = $"{thisbenchmarkResults} points";
        PrimeSpinner.IsRunning = false;
        PrimeSpinner.IsVisible = false;

        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }

        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.Prime,
            BenchmarkType = BenchmarkModel.Type.Partial,
            Result = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }

    private async void MatrixBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running MMUL benchmark...";
        }

        MatrixSpinner.IsRunning = true;
        MatrixSpinner.IsVisible = true;

        var thisbenchmarkResults = 0;
        Benchmarks.MatrixMultiplicationBenchmark? matrixBenchmark = null;

        await Task.Run(() =>
        {
            matrixBenchmark = new Benchmarks.MatrixMultiplicationBenchmark();
            thisbenchmarkResults = matrixBenchmark.MultiplyMatrix();
        });

        Results.Add(thisbenchmarkResults);
        MatrixResultsLabel.Text = $"{thisbenchmarkResults} points";
        MatrixSpinner.IsRunning = false;
        MatrixSpinner.IsVisible = false;

        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }

        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.MatrixMultiplication,
            BenchmarkType = BenchmarkModel.Type.Partial,
            Result = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }

    private async void MemoryBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running Memory benchmark...";
        }

        MemorySpinner.IsRunning = true;
        MemorySpinner.IsVisible = true;
        string thisbenchmarkResults = "";
        Benchmarks.MemoryBenchmark? memoryBenchmark = null;

        await Task.Run(() =>
        {
            memoryBenchmark = new Benchmarks.MemoryBenchmark();
            thisbenchmarkResults = memoryBenchmark.MTMemBandwidth();
        });

        //Results.Add(thisbenchmarkResults);
        MemoryResultsLabel.Text = thisbenchmarkResults;
        MemorySpinner.IsRunning = false;
        MemorySpinner.IsVisible = false;

        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }

        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.MemoryBandwidth,
            BenchmarkType = BenchmarkModel.Type.Partial,
            BandwidthResult = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }

    private async void AllBenchmarkButton_Clicked(object sender, EventArgs e)
    {
        Button? button = sender as Button;
        if (button != null)
        {
            button.IsEnabled = false;
            button.Text = "Running All benchmarks...";
        }

        AllSpinner.IsRunning = true;
        AllSpinner.IsVisible = true;
        var thisbenchmarkResults = 0;

        Benchmarks.CombinedBenchmark? combinedBenchmark = null;

        await Task.Run(() =>
        {
            combinedBenchmark = new Benchmarks.CombinedBenchmark();
            thisbenchmarkResults = (int)combinedBenchmark.RunAllBenchmarks();
        });

        Results.Add(thisbenchmarkResults);
        AllResultsLabel.Text = $"{thisbenchmarkResults} points";
        AllSpinner.IsRunning = false;
        AllSpinner.IsVisible = false;
        if (button != null)
        {
            button.IsEnabled = true;
            button.Text = "Start Benchmark";
        }
        await database.SaveItemAsync(new BenchmarkModel
        {
            Timestamp = DateTime.Now,
            TestNameValue = BenchmarkModel.TestName.Full,
            BenchmarkType = BenchmarkModel.Type.Full,
            Result = thisbenchmarkResults,
            Version = $"v{VersionTracking.Default.CurrentVersion}",
        });
    }
}