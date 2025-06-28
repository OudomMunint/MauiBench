using Hardware.Info;
using Microsoft.Maui.Graphics;
using NvAPIWrapper;
using NvAPIWrapper.GPU;
using SharpDX.DXGI;
using System.Management;
using System.Runtime.InteropServices;

namespace MauiBench.Pages;

public partial class SystemInfo : ContentPage
{
    private bool HasCpuSpecsBeenGathered = false;

    private bool HasGpuSpecsBeenGathered = false;

    private bool HasMemorySpecsBeenGathered = false;

    public SystemInfo()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Task.Delay(500).ContinueWith(_ =>
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                CpuSpinner.IsVisible = true;
                GpuSpinner.IsVisible = true;
                MemSpinner.IsVisible = true;
                GetGpuInfo();
                GetCpuInfo();
                GetMemoryInfo();
                CpuSpinner.IsVisible = false;
                GpuSpinner.IsVisible = false;
                MemSpinner.IsVisible = false;
            });
        });
    }

    private void GetGpuInfo()
    {
        if (HasGpuSpecsBeenGathered)
        {
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AddGpuInfoItem("GPU Information not available on this platform", isHeader: true);
            return;
        }

        HasGpuSpecsBeenGathered = true;
        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
        bool hasNvidiaGPU = false;

        try
        {
            hasNvidiaGPU = PhysicalGPU.GetPhysicalGPUs().Any(gpu => gpu.FullName.Contains("NVIDIA"));
        }
        catch
        {
            // NVIDIA API may not be available
        }

        if (hasNvidiaGPU)
        {
            try
            {
                NVIDIA.Initialize();
                var nvidiaGPUs = PhysicalGPU.GetPhysicalGPUs();
                var driver = NVIDIA.DriverVersion;
                var driverbranch = NVIDIA.DriverBranchVersion;

                foreach (var gpu in nvidiaGPUs)
                {
                    AddGpuInfoItem("NVIDIA", isHeader: true);
                    AddGpuInfoItem($"Type: {gpu.GPUType}");
                    AddGpuInfoItem($"GPU: {gpu.FullName}");
                    AddGpuInfoItem($"Core: {gpu.ArchitectInformation.ShortName}");
                    AddGpuInfoItem($"Shaders: {gpu.ArchitectInformation.NumberOfCores}");
                    AddGpuInfoItem($"ROPs: {gpu.ArchitectInformation.NumberOfROPs}");

                    var graphicsClockMHz = gpu.BoostClockFrequencies.GraphicsClock.Frequency / 1000;
                    AddGpuInfoItem($"Core Clock: {graphicsClockMHz} MHz");

                    var peakThroughput = gpu.ArchitectInformation.NumberOfCores * 2 * gpu.BoostClockFrequencies.GraphicsClock.Frequency / 1_000_000_000.0;
                    AddGpuInfoItem($"FP32 Throughput: {peakThroughput:F2} TFLOPS");

                    AddGpuInfoItem($"VRAM: {gpu.MemoryInformation.DedicatedVideoMemoryInkB / 1024} MB");
                    AddGpuInfoItem($"Type: {gpu.MemoryInformation.RAMType}");

                    var memoryClockMHz = gpu.BoostClockFrequencies.MemoryClock.Frequency / 1000;
                    AddGpuInfoItem($"VRAM Clock: {memoryClockMHz} MHz");
                    AddGpuInfoItem($"Bus Width: {gpu.MemoryInformation.FrameBufferBandwidth} bits");

                    var bandwidth = memoryClockMHz * 2 * gpu.MemoryInformation.FrameBufferBandwidth / 8000.0;
                    AddGpuInfoItem($"Bandwidth: {bandwidth} GB/s");

                    AddDivider(GpuInfoContainer);
                }
            }
            catch (Exception ex)
            {
                AddGpuInfoItem($"Error retrieving NVIDIA GPU info: {ex.Message}");
            }
        }

        foreach (var item in searcher.Get())
        {
            var manufacturer = item["AdapterCompatibility"]?.ToString();
            if (manufacturer == null) continue;

            if (manufacturer.ToLower().Contains("nvidia")) continue;

            if (manufacturer.ToLower().Contains("intel") || manufacturer.ToLower().Contains("amd"))
            {
                AddGpuInfoItem(manufacturer, isHeader: true);
                AddGpuInfoItem($"Name: {item["Name"]}");
                AddGpuInfoItem($"Manufacturer: {manufacturer}");
                AddGpuInfoItem($"VRAM: {Convert.ToUInt64(item["AdapterRAM"]) / (1024 * 1024)} MB");
                AddDivider(GpuInfoContainer);
            }
            else
            {
                if (manufacturer.ToLower().Contains("advanced micro devices"))
                {
                    manufacturer = "AMD";
                }

                if (!hasNvidiaGPU)
                {
                    try
                    {
                        using var factory = new Factory1();
                        using var adapter = factory.GetAdapter(0);
                        var desc = adapter.Description;

                        AddGpuInfoItem(manufacturer, isHeader: true);
                        AddGpuInfoItem($"Name: {desc.Description}");
                        AddGpuInfoItem($"Manufacturer: {manufacturer}");
                        AddGpuInfoItem($"Driver Version: {item["DriverVersion"]}");

                        if (desc.DedicatedVideoMemory == 0)
                        {
                            AddGpuInfoItem("No dedicated GPU memory found");
                        }
                        else
                        {
                            AddGpuInfoItem($"VRAM: {desc.DedicatedVideoMemory / (1024 * 1024)} MB");
                            AddGpuInfoItem($"Shared Memory: {desc.SharedSystemMemory / (1024 * 1024)} MB");
                        }

                        AddDivider(GpuInfoContainer);
                    }
                    catch (Exception ex)
                    {
                        AddGpuInfoItem($"Error retrieving GPU info: {ex.Message}");
                    }
                }
            }
        }
    }

    private void GetCpuInfo()
    {
        if (HasCpuSpecsBeenGathered)
        {
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AddCpuInfoItem("CPU Information not available on this platform", isHeader: true);
            return;
        }

        try
        {
            HasCpuSpecsBeenGathered = true;
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (var item in searcher.Get())
            {
                try
                {
                    string cpuName = item["Name"]?.ToString() ?? "";
                    string headerText = "CPU Information";

                    if (cpuName.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                    {
                        headerText = "Intel";
                    }
                    else if (cpuName.Contains("AMD", StringComparison.OrdinalIgnoreCase) ||
                             cpuName.Contains("Ryzen", StringComparison.OrdinalIgnoreCase))
                    {
                        headerText = "AMD";
                    }
                    else if (cpuName.Contains("Apple", StringComparison.OrdinalIgnoreCase))
                    {
                        headerText = "Apple";
                    }

                    AddCpuInfoItem(headerText, isHeader: true);
                    AddCpuInfoItem($"CPU: {item["Name"]}");
                    AddCpuInfoItem($"Cores: {item["NumberOfCores"]}, Threads: {item["NumberOfLogicalProcessors"]}");
                    AddCpuInfoItem($"Base Clock: {item["MaxClockSpeed"]} MHz");

                    IHardwareInfo hardwareInfo = new HardwareInfo();
                    hardwareInfo.RefreshCPUList();

                    foreach (var cpu in hardwareInfo.CpuList)
                    {
                        var L1DataCacheSize = cpu.L1DataCacheSize / 1024;
                        var L1InstructionCacheSize = cpu.L1InstructionCacheSize / 1024;

                        AddCpuInfoItem($"Clock Speed: {cpu.CurrentClockSpeed} MHz");
                        AddCpuInfoItem($"L1 Data Cache: {L1DataCacheSize} KB");
                        AddCpuInfoItem($"L1 Instruction Cache: {L1InstructionCacheSize} KB");
                    }

                    AddCpuInfoItem($"L2 Cache: {Convert.ToInt64(item["L2CacheSize"]) / 1024} MB");
                    AddCpuInfoItem($"L3 Cache: {Convert.ToInt64(item["L3CacheSize"]) / 1024} MB");
                }
                catch (Exception ex)
                {
                    AddCpuInfoItem($"Error: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            AddCpuInfoItem($"Error retrieving CPU info: {ex.Message}");
        }
    }

    private void GetMemoryInfo()
    {
        if (HasMemorySpecsBeenGathered)
        {
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AddMemInfoItem("Memory Information not available on this platform", isHeader: true);
            return;
        }

        try
        {
            HasMemorySpecsBeenGathered = true;
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
            var modules = searcher.Get().Cast<ManagementObject>().ToList();

            if (modules.Count == 0)
            {
                AddMemInfoItem("No memory modules detected");
                return;
            }

            // Aggregate summary data
            long totalCapacity = 0;
            string? manufacturer = null;
            int speed = 0;
            int busWidth = 0;

            foreach (var module in modules)
            {
                totalCapacity += Convert.ToInt64(module["Capacity"]);
                if (manufacturer is null)
                    manufacturer = module["Manufacturer"]?.ToString()?.Trim();

                if (speed == 0 && int.TryParse(module["Speed"]?.ToString()?.Trim(), out var parsedSpeed))
                    speed = parsedSpeed;

                if (busWidth == 0 && int.TryParse(module["DataWidth"]?.ToString(), out var parsedBusWidth))
                    busWidth = parsedBusWidth;
            }

            double peakBandwidth = speed * 2 * busWidth / 8000.0;

            int slot = 1;
            foreach (var module in modules)
            {
                AddMemInfoItem($"Slot {slot++}", isHeader: true);

                if (module["Speed"]?.ToString() is string s && s != "0")
                    AddMemInfoItem($"Speed: {s.Trim()} MHz");

                var cap = Convert.ToInt64(module["Capacity"]);
                AddMemInfoItem($"Capacity: {cap / (1024 * 1024 * 1024)} GB");
                AddDivider(MemoryInfoContainer);
            }

            // Summary
            AddMemInfoItem($"Manufacturer: {manufacturer ?? "Unknown"}");
            AddMemInfoItem($"Total Capacity: {totalCapacity / (1024 * 1024 * 1024)} GB");
            AddMemInfoItem($"Memory Speed: {speed} MHz");
            AddMemInfoItem($"Bus Width: {busWidth} Bits");
            AddMemInfoItem($"Peak Bandwidth: {peakBandwidth:F2} GB/s");
        }
        catch (Exception ex)
        {
            AddMemInfoItem($"Error retrieving memory info: {ex.Message}");
        }
    }

    private void AddGpuInfoItem(string text, bool isHeader = false)
    {
        var label = new Label
        {
            Text = text,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            FontSize = isHeader ? 16 : 14,
            Margin = new Thickness(isHeader ? 0 : 5, 2, 0, 2)
        };

        GpuInfoContainer.Add(label);
    }

    private void AddCpuInfoItem(string text, bool isHeader = false)
    {
        var label = new Label
        {
            Text = text,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            FontSize = isHeader ? 16 : 14,
            Margin = new Thickness(isHeader ? 0 : 5, 2, 0, 2)
        };

        CpuInfoContainer.Add(label);
    }

    private void AddDivider(VerticalStackLayout parent)
    {
        var boxView = new BoxView
        {
            HeightRequest = 1,
            Color = Colors.LightGray,
            Margin = new Thickness(0, 10, 0, 10),
            HorizontalOptions = LayoutOptions.Fill
        };

        parent.Add(boxView);
    }

    private void AddMemInfoItem(string text, bool isHeader = false)
    {
        var label = new Label
        {
            Text = text,
            FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
            FontSize = isHeader ? 16 : 14,
            Margin = new Thickness(isHeader ? 0 : 5, 2, 0, 2)
        };

        MemoryInfoContainer.Add(label);
    }
}