# Scanner Automation Scripts

Scanner automation scripts in C# (.NET 8) and Python - CLI wrappers for NAPS2 with auto-detection, named parameters, and multi-format output.

## What it does

These scripts provide a streamlined command-line interface for document scanning using NAPS2 as the backend. Both scripts offer identical functionality: automatic scanner detection, support for WIA and TWAIN drivers, and can save scans in multiple formats (PNG, JPEG, TIFF, BMP, PDF) with customizable resolution and color settings.

### Key Features

- ✅ **Two implementations** - C# (.NET 8) and Python 3.7+
- ✅ **Identical functionality** - Same parameters and behavior
- ✅ **Automatic scanner detection** - No need to remember device names
- ✅ **Multiple output formats** - PNG, JPEG, TIFF, BMP, PDF
- ✅ **Flexible DPI settings** - From 75 to 1200+ DPI
- ✅ **Color mode options** - Color, grayscale, black & white
- ✅ **ADF and flatbed support** - Works with document feeders and glass beds
- ✅ **Batch scanning** - Multiple pages automatically numbered
- ✅ **Cross-platform** - Both scripts work on Windows, macOS, and Linux

## Files in this Repository

- **`scanner.cs`** - C# (.NET 8) implementation
- **`scanner.py`** - Python implementation (identical functionality)
- **`README.md`** - This documentation

## Prerequisites

### Common Requirements

#### NAPS2 Scanner Software
Download and install NAPS2 from:
- **Official website**: https://www.naps2.com/download
- **Recommended version**: 8.1.4 or newer

**Installation Steps:**
1. Download `naps2-8.1.4-win-x64.exe` (or appropriate version for your OS)
2. Run the installer with default settings
3. **Important**: Make sure NAPS2 is added to your system PATH

**Verify NAPS2 installation:**
```bash
NAPS2.Console --version
# Should show version information
```

#### Scanner Drivers
Ensure your scanner has proper drivers installed:
- **WIA drivers** (Windows Image Acquisition) - Usually installed automatically
- **TWAIN drivers** (optional) - Often more reliable for business scanners

### For C# Script (scanner.cs)

#### .NET 8 Runtime
Download and install from: https://dotnet.microsoft.com/download/dotnet/8.0

**Verify installation:**
```bash
dotnet --version
# Should show 8.0.x or higher
```

### For Python Script (scanner.py)

#### Python 3.7+
Download from: https://www.python.org/downloads/

**Verify installation:**
```bash
python --version
# Should show 3.7.x or higher
```

**No additional packages required** - uses only Python standard library.

## Usage

Both scripts have identical command-line interfaces:

### Basic Syntax
```bash
# C# version
dotnet run scanner.cs [options]

# Python version  
python scanner.py [options]
```

### Command Line Options

| Option | Short | Description | Default |
|--------|-------|-------------|---------|
| `--output <folder>` | `-o` | Output folder path | `scanned_pages` |
| `--prefix <name>` | `-p` | File name prefix | `page` |
| `--format <ext>` | `-f` | Output format (png, jpg, tiff, bmp, pdf) | `png` |
| `--dpi <number>` | `-d` | Scanning resolution | `300` |
| `--color <mode>` | `-c` | Color mode (color, gray, bw) | `color` |
| `--source <src>` | `-s` | Paper source (feeder, glass) | `feeder` |
| `--device <name>` | | Specific scanner device name | auto-detect |
| `--driver <type>` | | Scanner driver (wia, twain) | `wia` |
| `--help` | `-h` | Show help information | |

## Examples

All examples work identically with both scripts (just change the command):

### Basic Examples

**Simple scan with defaults:**
```bash
# C# version
dotnet run scanner.cs

# Python version
python scanner.py

# Output: scanned_pages/page_0001.png (300 DPI, color, ADF)
```

**Show help:**
```bash
python scanner.py --help
dotnet run scanner.cs --help
```

### Custom Output Examples

**Scan to specific folder:**
```bash
python scanner.py --output "D:\Documents\Scans"
# Output: D:\Documents\Scans\page_0001.png
```

**Custom file prefix and format:**
```bash
python scanner.py --prefix "invoice" --format jpg --output "C:\Invoices"
# Output: C:\Invoices\invoice_0001.jpg, invoice_0002.jpg, etc.
```

### Quality and Format Examples

**High-resolution scanning:**
```bash
python scanner.py --dpi 600 --format tiff --prefix "document_hq"
# Output: scanned_pages/document_hq_0001.tiff (600 DPI color TIFF)
```

**Web-optimized scanning:**
```bash
python scanner.py --dpi 150 --format jpg --color gray --prefix "web"
# Output: scanned_pages/web_0001.jpg (150 DPI grayscale JPEG)
```

**PDF multi-page document:**
```bash
python scanner.py --format pdf --prefix "contract" --color bw
# Output: scanned_pages/contract.pdf (all pages in one PDF)
```

### Driver and Device Examples

**Specific scanner device:**
```bash
python scanner.py --device "Xerox WIA - ETE84DEC0F2B58" --source feeder
```

**Use TWAIN driver:**
```bash
python scanner.py --driver twain --format pdf --prefix "report"
```

**Flatbed scanning:**
```bash
python scanner.py --source glass --dpi 600 --format png --prefix "photo"
```

### Workflow Examples

**Daily document scanning:**
```bash
# Windows
python scanner.py --prefix "daily_%DATE%" --format pdf

# Linux/macOS  
python scanner.py --prefix "daily_$(date +%Y%m%d)" --format pdf
```

**Invoice processing:**
```bash
python scanner.py --output "C:\Accounting\Invoices" --prefix "inv" --format jpg --color gray --dpi 300
```

**Legal document archiving:**
```bash
python scanner.py --output "D:\Legal\Cases\2024" --prefix "case_001" --format tiff --dpi 600 --color gray
```

## Running the Scripts

### C# Script
```bash
# Option 1: Direct execution with dotnet
dotnet run scanner.cs --output "D:\scans" --format jpg

# Option 2: Compile first, then run
dotnet build scanner.cs
./scanner --dpi 600 --color gray
```

### Python Script
```bash
# Direct execution
python scanner.py --output "D:\scans" --format jpg

# Make executable (Linux/macOS)
chmod +x scanner.py
./scanner.py --dpi 600 --color gray
```

## Troubleshooting

### Scanner Not Found
```bash
# List available scanners
NAPS2.Console --driver wia --listdevices
NAPS2.Console --driver twain --listdevices

# Try different driver
python scanner.py --driver twain
```

### NAPS2 Not Found Error
1. Verify NAPS2 installation: `NAPS2.Console --version`
2. Add NAPS2 to PATH environment variable
3. Reinstall NAPS2 with default settings

### Script-Specific Issues

**C# Script:**
- Ensure .NET 8 runtime is installed
- Check that `dotnet` command is available in PATH

**Python Script:**
- Ensure Python 3.7+ is installed
- Check that `python` command works in terminal

### Scanner Issues

**ADF/Feeder Problems:**
```bash
# Try flatbed instead
python scanner.py --source glass

# Try TWAIN driver for better ADF support
python scanner.py --driver twain --source feeder
```

**Poor Scan Quality:**
```bash
# Increase DPI
python scanner.py --dpi 600

# Use uncompressed format
python scanner.py --format tiff
```

## Output Formats

- **PNG** - Best quality, lossless compression, moderate file size
- **JPEG** - Good quality, lossy compression, smallest file size  
- **TIFF** - Professional quality, optional compression, large file size
- **BMP** - Uncompressed, largest file size, maximum quality
- **PDF** - Multi-page document format, good for archiving

## DPI Recommendations

- **75-150 DPI** - Web use, email attachments
- **300 DPI** - Standard document scanning, printing
- **600 DPI** - High quality documents, small text
- **1200+ DPI** - Professional archival, photography

## Technical Notes

### C# Script Features
- **Modern .NET 8** - Top-level statements, async/await, nullable reference types
- **Fast startup** - Optimized for performance
- **Cross-platform** - Runs on Windows, macOS, Linux

### Python Script Features  
- **Python 3.7+** - Uses modern asyncio features
- **Standard library only** - No external dependencies
- **Cross-platform** - Works anywhere Python runs

### Both Scripts
- **Identical behavior** - Same parameters, same output, same error handling
- **Async operations** - Non-blocking I/O for better performance
- **Automatic file detection** - Verifies scan success by checking created files
- **Real-time output** - Shows NAPS2 progress during scanning

## Contributing

1. Fork the repository
2. Make your changes to the scripts
3. Test with your scanner setup
4. Submit a pull request

Both scripts should maintain identical functionality when changes are made.

## License

This project is licensed under the MIT License.

## Support

- **Issues**: Report bugs and feature requests on GitHub
- **NAPS2 Support**: Visit https://www.naps2.com for NAPS2-specific issues
- **Scanner Drivers**: Contact your scanner manufacturer for driver support
