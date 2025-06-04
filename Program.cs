using System.Diagnostics;

// Default parameters
var outputFolder = "scanned_pages";
var filePrefix = "page";
var fileFormat = "png";
var dpi = 300;
var colorMode = "color"; // color, gray, bw
var source = "feeder"; // feeder, glass
string? deviceName = null; // auto-detect if null
var driver = "wia"; // wia, twain

// Parse command line arguments
for (int i = 0; i < args.Length; i++)
{
    switch (args[i].ToLower())
    {
        case "-o" or "--output":
            if (i + 1 < args.Length) outputFolder = args[++i];
            break;
        case "-p" or "--prefix":
            if (i + 1 < args.Length) filePrefix = args[++i];
            break;
        case "-f" or "--format":
            if (i + 1 < args.Length) fileFormat = args[++i];
            break;
        case "-d" or "--dpi":
            if (i + 1 < args.Length && int.TryParse(args[i + 1], out int parsedDpi))
            {
                dpi = parsedDpi;
                i++;
            }
            break;
        case "-c" or "--color":
            if (i + 1 < args.Length) colorMode = args[++i];
            break;
        case "-s" or "--source":
            if (i + 1 < args.Length) source = args[++i];
            break;
        case "--device":
            if (i + 1 < args.Length) deviceName = args[++i];
            break;
        case "--driver":
            if (i + 1 < args.Length) driver = args[++i];
            break;
        case "-h" or "--help":
            ShowHelp();
            return;
    }
}

Console.WriteLine("=== MODERN SCANNER WRAPPER ===");
Console.WriteLine($"Output: {outputFolder}");
Console.WriteLine($"Prefix: {filePrefix}");
Console.WriteLine($"Format: {fileFormat}");
Console.WriteLine($"DPI: {dpi}");
Console.WriteLine($"Color: {colorMode}");
Console.WriteLine($"Source: {source}");
Console.WriteLine($"Device: {deviceName ?? "auto-detect"}");
Console.WriteLine($"Driver: {driver}");
Console.WriteLine();

// Execute scan
var success = await ScanWithNAPS2(outputFolder, filePrefix, fileFormat, dpi, colorMode, source, deviceName, driver);

Console.WriteLine(success ? "✓ Scan completed successfully!" : "✗ Scan failed.");
Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

/// <summary>
/// Shows help information
/// </summary>
static void ShowHelp()
{
    Console.WriteLine("=== MODERN SCANNER WRAPPER - HELP ===");
    Console.WriteLine();
    Console.WriteLine("USAGE:");
    Console.WriteLine("  scanner [options]");
    Console.WriteLine();
    Console.WriteLine("OPTIONS:");
    Console.WriteLine("  -o, --output <folder>    Output folder (default: scanned_pages)");
    Console.WriteLine("  -p, --prefix <name>      File prefix (default: page)");
    Console.WriteLine("  -f, --format <ext>       Format: png, jpg, tiff, bmp, pdf (default: png)");
    Console.WriteLine("  -d, --dpi <number>       DPI resolution (default: 300)");
    Console.WriteLine("  -c, --color <mode>       Color mode: color, gray, bw (default: color)");
    Console.WriteLine("  -s, --source <src>       Source: feeder, glass (default: feeder)");
    Console.WriteLine("  --device <name>          Scanner device name (auto-detect if not specified)");
    Console.WriteLine("  --driver <type>          Driver: wia, twain (default: wia)");
    Console.WriteLine("  -h, --help               Show this help");
    Console.WriteLine();
    Console.WriteLine("EXAMPLES:");
    Console.WriteLine("  scanner");
    Console.WriteLine("    Basic scan with auto-detection");
    Console.WriteLine();
    Console.WriteLine("  scanner -o \"D:\\scans\" -p \"doc\" -f jpg -d 600 -c gray");
    Console.WriteLine("    Custom folder, prefix, format, DPI and color");
    Console.WriteLine();
    Console.WriteLine("  scanner --device \"Xerox WIA - ETE84DEC0F2B58\" --source feeder");
    Console.WriteLine("    Specific device and source");
    Console.WriteLine();
    Console.WriteLine("  scanner --driver twain --format pdf --dpi 300");
    Console.WriteLine("    Use TWAIN driver, save as PDF");
}

/// <summary>
/// Executes scan using NAPS2
/// </summary>
/// <param name="outputFolder">Destination folder</param>
/// <param name="filePrefix">File name prefix</param>
/// <param name="fileFormat">File format (png, jpg, tiff, bmp, pdf)</param>
/// <param name="dpi">DPI resolution</param>
/// <param name="colorMode">Color mode (color, gray, bw)</param>
/// <param name="source">Source (feeder, glass)</param>
/// <param name="deviceName">Scanner device name (null for auto-detect)</param>
/// <param name="driver">Driver type (wia, twain)</param>
/// <returns>True if scan succeeded</returns>
static async Task<bool> ScanWithNAPS2(string outputFolder, string filePrefix, string fileFormat,
                                      int dpi, string colorMode, string source, string? deviceName, string driver)
{
    try
    {
        // Create output folder if it doesn't exist
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            Console.WriteLine($"Created folder: {outputFolder}");
        }

        // Check if NAPS2 is available in PATH
        if (!await IsNAPS2Available())
        {
            Console.WriteLine("ERROR: NAPS2.Console not found in system PATH.");
            Console.WriteLine("Install NAPS2 from: https://www.naps2.com/download");
            Console.WriteLine("Or add NAPS2 installation folder to PATH environment variable.");
            return false;
        }

        // Find scanner device if not specified
        deviceName ??= await GetScannerDevice(driver);
        if (string.IsNullOrEmpty(deviceName))
        {
            Console.WriteLine("ERROR: No scanner found.");
            return false;
        }

        Console.WriteLine($"Scanner: {deviceName}");

        // Build output path
        var outputPath = fileFormat.ToLower() == "pdf"
            ? Path.Combine(outputFolder, $"{filePrefix}.{fileFormat}")
            : Path.Combine(outputFolder, $"{filePrefix}_$(nnnn).{fileFormat}");

        // Build NAPS2 command arguments
        var arguments = $"--driver {driver} " +
                       $"--device \"{deviceName}\" " +
                       $"--source {source} " +
                       $"--dpi {dpi} " +
                       $"--bitdepth {colorMode} " +
                       $"--output \"{outputPath}\" " +
                       $"--verbose";

        Console.WriteLine();
        Console.WriteLine("NAPS2 Command:");
        Console.WriteLine($"NAPS2.Console {arguments}");
        Console.WriteLine();
        Console.WriteLine("Starting scan...");
        Console.WriteLine("MAKE SURE PAPER IS IN THE ADF TRAY!");
        Console.WriteLine();

        // Execute NAPS2
        var processInfo = new ProcessStartInfo
        {
            FileName = "NAPS2.Console",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = false
        };

        using var process = new Process { StartInfo = processInfo };

        // Handle output in real-time
        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"NAPS2: {e.Data}");
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"ERROR: {e.Data}");
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        Console.WriteLine();
        Console.WriteLine($"NAPS2 finished with exit code: {process.ExitCode}");

        // Check if files were created (more reliable than exit code)
        var hasFiles = CheckForScannedFiles(outputFolder, filePrefix, fileFormat);

        if (hasFiles || process.ExitCode == 0)
        {
            ShowScanResults(outputFolder, filePrefix, fileFormat);
            return true;
        }

        Console.WriteLine("No files were created.");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during scan: {ex.Message}");
        return false;
    }
}

/// <summary>
/// Checks if NAPS2.Console is available in system PATH
/// </summary>
/// <returns>True if NAPS2 is available</returns>
static async Task<bool> IsNAPS2Available()
{
    try
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "NAPS2.Console",
            Arguments = "--version",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode == 0;
    }
    catch
    {
        return false;
    }
}

/// <summary>
/// Gets available scanner device
/// </summary>
/// <param name="driver">Driver type (wia, twain)</param>
/// <returns>Scanner device name or null if not found</returns>
static async Task<string?> GetScannerDevice(string driver)
{
    try
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "NAPS2.Console",
            Arguments = $"--driver {driver} --listdevices",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode == 0 && !string.IsNullOrWhiteSpace(output))
        {
            return output.Trim().Split('\n')[0].Trim();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error finding scanner: {ex.Message}");
    }

    return null;
}

/// <summary>
/// Checks if scanned files exist in the output folder
/// </summary>
/// <param name="outputFolder">Output folder path</param>
/// <param name="filePrefix">File prefix to search for</param>
/// <param name="fileFormat">File format extension</param>
/// <returns>True if files were found</returns>
static bool CheckForScannedFiles(string outputFolder, string filePrefix, string fileFormat)
{
    try
    {
        if (!Directory.Exists(outputFolder))
            return false;

        var searchPattern = $"{filePrefix}*.{fileFormat}";
        var files = Directory.GetFiles(outputFolder, searchPattern);

        return files.Length > 0;
    }
    catch
    {
        return false;
    }
}

/// <summary>
/// Shows scan results summary
/// </summary>
/// <param name="outputFolder">Output folder path</param>
/// <param name="filePrefix">File prefix</param>
/// <param name="fileFormat">File format</param>
static void ShowScanResults(string outputFolder, string filePrefix, string fileFormat)
{
    try
    {
        var searchPattern = $"{filePrefix}*.{fileFormat}";
        var files = Directory.GetFiles(outputFolder, searchPattern);

        Console.WriteLine($"\nFiles created ({files.Length}):");
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var sizeKb = fileInfo.Length / 1024;
            Console.WriteLine($"  {Path.GetFileName(file)} ({sizeKb:N0} KB)");
        }

        if (files.Length > 0)
        {
            Console.WriteLine($"\nAll files saved to: {Path.GetFullPath(outputFolder)}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error showing results: {ex.Message}");
    }
}