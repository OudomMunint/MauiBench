using NvAPIWrapper;
using NvAPIWrapper.GPU;
using SharpDX.DXGI;
using System.Management;
using System.Runtime.InteropServices;

namespace MauiBench
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            GetGpuInfo();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void GetGpuInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                bool hasNvidiaGPU = PhysicalGPU.GetPhysicalGPUs().Any(gpu => gpu.FullName.Contains("NVIDIA"));

                if (hasNvidiaGPU)
                {
                    NVIDIA.Initialize();
                    var nvidiaGPUs = PhysicalGPU.GetPhysicalGPUs();
                    var driver = NVIDIA.DriverVersion;
                    var driverbranch = NVIDIA.DriverBranchVersion;

                    foreach (var gpu in nvidiaGPUs)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("[GPU Information]");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("GPU Type: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(gpu.GPUType);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Name: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(gpu.FullName);
                        GpuName.Text = gpu.FullName;

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("GPU Core: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(gpu.ArchitectInformation.ShortName);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Shaders: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(gpu.ArchitectInformation.NumberOfCores);

                        var graphicsClockMHz = gpu.BoostClockFrequencies.GraphicsClock.Frequency / 1000;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("GPU Core Speed: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("{0} MHz", graphicsClockMHz);
                        GpuClock.Text = $"{graphicsClockMHz} MHz";

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("VRAM: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("{0} MB", gpu.MemoryInformation.DedicatedVideoMemoryInkB / 1024);
                        GpuMem.Text = $"{gpu.MemoryInformation.DedicatedVideoMemoryInkB / 1024} MB";

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("VRAM Type: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(gpu.MemoryInformation.RAMType);

                        var memoryClockMHz = gpu.BoostClockFrequencies.MemoryClock.Frequency / 1000;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("VRAM Frequency: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("{0} MHz", memoryClockMHz);
                    }
                }

                foreach (var item in searcher.Get())
                {
                    var manufacturer = item["AdapterCompatibility"]?.ToString();
                    if (manufacturer == null) continue;

                    if (manufacturer.ToLower().Contains("nvidia")) continue;

                    if (manufacturer.ToLower().Contains("intel") || manufacturer.ToLower().Contains("amd"))
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("-----------------------------------------------------------");

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("[Integrated GPU]");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Name: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(item["Name"]);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Manufacturer: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(manufacturer);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("Driver Version: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(item["DriverVersion"]);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("VRAM: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("{0} MB", Convert.ToUInt64(item["AdapterRAM"]) / (1024 * 1024));

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("-----------------------------------------------------------");
                    }
                    else
                    {
                        if (manufacturer.ToLower().Contains("advanced micro devices"))
                        {
                            manufacturer = "AMD";
                        }

                        if (!hasNvidiaGPU)
                        {
                            using var factory = new Factory1();
                            using var adapter = factory.GetAdapter(0);
                            var desc = adapter.Description;

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Name: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(desc.Description);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Manufacturer: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(manufacturer);

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Driver Version: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(item["DriverVersion"]);

                            if (desc.DedicatedVideoMemory == 0)
                            {
                                Console.WriteLine("No dedicated GPU memory found");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("VRAM: ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("{0} MB", desc.DedicatedVideoMemory / (1024 * 1024));

                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write("Shared Memory: ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("{0} MB", desc.SharedSystemMemory / (1024 * 1024));
                            }

                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("-----------------------------------------------------------");
                        }
                    }
                }
            }
        }
    }
}