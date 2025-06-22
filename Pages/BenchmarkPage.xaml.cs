using MauiBench.Data;
using MauiBench.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiBench.Pages;

public partial class BenchmarkPage : ContentPage
{
    public List<string> Results { get; set; } = new List<string>();

    public ItemDatabase database;

    public BenchmarkPage()
    {
        InitializeComponent();
        InitializeDatabaseAsync();
    }

    private async void InitializeDatabaseAsync()
    {
        database = await ItemDatabase.Instance;
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
            Result = thisbenchmarkResults
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
            Result = thisbenchmarkResults
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
            Result = thisbenchmarkResults
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
            Result = thisbenchmarkResults
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

        Results.Add(thisbenchmarkResults);
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
            Result = thisbenchmarkResults
        });
    }
}